using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.ExceptionProcessors;
using yourLogs.Exceptions.Core.LogWriters.Providers;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Core.Tests.ExceptionProcessors;
using yourLogs.Exceptions.Db.Ef;
using yourLogs.Exceptions.Db.Ef.LogWriters;
using yourLogs.Exceptions.Db.Ef.Models;
using yourLogs.Exceptions.Db.Ef.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class DbPersistenceHandlerTests
{
    private readonly DbContextOptions inMemoryOption;
    private readonly ErrorDbContext context;
    private EfCoreRepository repository;

    public DbPersistenceHandlerTests()
    {
        inMemoryOption =
            new DbContextOptionsBuilder()
                .UseInMemoryDatabase("DbPersistenceHandlerTest")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;

        context = new ErrorDbContext(inMemoryOption);
        context.Database.EnsureCreated();
        repository = new EfCoreRepository(() => new ErrorDbContext(inMemoryOption));
    }

    [SetUp]
    public void Setup()
    {
        foreach (var item in context.Logs)
        {
            context.Logs.Remove(item);
        }

        foreach (var item in context.Tags)
        {
            context.Tags.Remove(item);
        }

        context.SaveChanges();
        context.Tags.Add(new TagEntity()
        {
            Tag = "existing"
        });
        context.SaveChanges();
    }

    [Test]
    public async Task StoreSingleException()
    {
        var handler = DbLogWriter.Create(repository, ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogErrorAsync(new Exception("hi"));

        var result = await context.Logs.FirstAsync(e => e.Code.Equals("hi"));
        Assert.AreEqual(Environment.MachineName, result.Source);
        Assert.AreEqual(LogType.Error, result.LogType);
    }

    [Test]
    public async Task StoreNestedException()
    {
        var handler = DbLogWriter.Create(repository, ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogWarnAsync(
            yourLogs.Exceptions.Core.Tests.ExceptionProcessors.ExceptionUtilities.GetNestedException("Inner", "Outer"));

        var result = await context.Logs.FirstAsync(e => e.Code.Equals("Outer"));
        Assert.AreEqual(Environment.MachineName, result.Source);
        Assert.AreEqual(LogType.Warning, result.LogType);
        Assert.IsTrue(result.Text.Contains("Outer"));
        Assert.IsTrue(result.Description.Contains("---"));
        Assert.IsTrue(result.Description.Contains("Inner"));
        Assert.IsTrue(result.Description.Contains(".cs"));
    }

    [Test]
    public async Task StoreSingleExceptionWithSourceProvider()
    {
        var handler = DbLogWriter.Create(
            repository,
            ExceptionTextProviders.Default,
            sourceProvider: (ex) => "myTest",
            codeProvider: (ex) => ex.Message);

        await handler.LogErrorAsync(new Exception("SourceProvider"));

        var result = await context.Logs.FirstAsync(e => e.Code.Equals("SourceProvider"));
        Assert.AreEqual("myTest", result.Source);
        Assert.AreEqual(LogType.Error, result.LogType);
    }

    [Test]
    public async Task StoreCustomLog()
    {
        var handler = DbLogWriter.Create(
            repository, ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogAsync(LogType.Custom, "log message", new {Value = 3});
        var result = await context.Logs.FirstAsync(e => e.Text.Equals("log message"));
        Assert.AreEqual(Environment.MachineName, result.Source);
        Assert.AreEqual(LogType.Custom, result.LogType);

        var loggedObject = JsonConvert.DeserializeObject<KeyValuePair<string, int>>(result.Description);
        Assert.AreEqual(3, loggedObject.Value);
    }

    [Test]
    public async Task GetAllFilters_LogType()
    {
        var handler = DbLogWriter.Create(
            repository,
            ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogAsync(LogType.Custom, "log message", new {Value = 3});
        await handler.LogAsync(LogType.Error, "error message", new {Value = 3});
        await handler.LogAsync(LogType.Warning, "warn message", new {Value = 3});
        await handler.LogAsync(LogType.Info, "info message", new {Value = 3});
        await handler.LogAsync(LogType.Trace, "trace message", new {Value = 3});

        PageInfo pageInfo = new PageInfo()
        {
            Page = 0,
            PageSize = 10,
        };

        var results = await repository.GetAll(pageInfo, new FilterParam()
        {
            LogTypes = new[] {LogType.Info, LogType.Custom}
        });

        Assert.AreEqual(2, results.Total);
        Assert.AreEqual(0, results.Page);

        var custom = results.Items.First(r => r.LogType == LogType.Custom);
        var info = results.Items.First(r => r.LogType == LogType.Info);
        Assert.IsTrue(custom.Text.Contains("log message"));
        Assert.IsTrue(info.Text.Contains("info message"));
    }

    [Test]
    public async Task GetAll_Paged()
    {
        var handler = DbLogWriter.Create(
            repository,
            ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        for (int i = 0; i < 100; i++)
        {
            await handler.LogAsync(LogType.Custom, "log message", i);
        }

        var pageInfo = PageInfo.Create(3, 10);
        var results = await repository.GetAll(pageInfo, new FilterParam());
        Assert.AreEqual(100, results.Total);
        Assert.AreEqual(10, results.Items.Count());
        // from 30 to 40
        // ordered by occurred date desc
        var resultList = results.Items.ToList();
        for (int i = 60; i < 70; i++)
        {
            var target = results.Items.ElementAt(9 - (i - 60));
            Assert.AreEqual(i.ToString(), target.Description);
        }
    }
    
    [Test]
    public async Task GetAllFilters_Date()
    {
        var handler = DbLogWriter.Create(
            repository,
            ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        DateTimeOffset before = DateTimeOffset.Now;

        await handler.LogAsync(LogType.Custom, "log message", new {Value = 3});
        await handler.LogAsync(LogType.Error, "error message", new {Value = 3});
        await handler.LogAsync(LogType.Warning, "warn message", new {Value = 3});
        await handler.LogAsync(LogType.Info, "info message", new {Value = 3});
        await handler.LogAsync(LogType.Trace, "trace message", new {Value = 3});

        DateTimeOffset after = DateTimeOffset.Now;

        PageInfo pageInfo = new PageInfo()
        {
            Page = 0,
            PageSize = 10,
        };

        var results = await repository.GetAll(pageInfo, new FilterParam()
        {
            Since = after
        });

        Assert.AreEqual(0, results.Total);

        results = await repository.GetAll(pageInfo, new FilterParam()
        {
            Since = before
        });

        Assert.AreEqual(5, results.Total);
    }

    [Test]
    public async Task InsertTags()
    {
        /*var tagRepository = new Ef5Repository(
            () => new ErrorDbContext(inMemoryOption));
        var test = (await tagRepository.GetTagsAsync()).ToList();*/
        
        var handler = DbLogWriter.Create(repository, ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        // await handler.Init();

        await handler.LogErrorAsync(ExceptionUtilities.GetNestedException("inner", "outer"), "existing", "hi");
        await handler.LogErrorAsync(ExceptionUtilities.GetNestedException("inner", "outer"), "hi");
        await handler.LogErrorAsync(ExceptionUtilities.GetNestedException("inner", "outer"), "existing");

        var justExisting = await repository.GetAll(
            PageInfo.Default(), new FilterParam()
            {
                Tags = new[] {"existing"}
            });

        var justHi = await repository.GetAll(
            PageInfo.Default(), new FilterParam()
            {
                Tags = new[] {"hi"}
            });

        var both = await repository.GetAll(
            PageInfo.Default(), new FilterParam()
            {
                Tags = new[] {"hi", "existing"}
            });

        var tags = await repository.GetTagsAsync();
        Assert.AreEqual(2, tags.Count());
        Assert.AreEqual(2, justHi.Total);
        Assert.AreEqual(2, justExisting.Total);
        Assert.AreEqual(3, both.Total);
    }
}