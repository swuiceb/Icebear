using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.ExceptionProcessors;
using Icebear.Exceptions.Core.LogWriters.Providers;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Db.Ef;
using Icebear.Exceptions.Db.Ef.LogWriters;
using Icebear.Exceptions.Db.Ef.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class DbPersistenceHandlerTests
{
    private readonly DbContextOptions inMemoryOption;
    private readonly ErrorDbContext context;

    public DbPersistenceHandlerTests()
    {
        inMemoryOption =
            new DbContextOptionsBuilder()
                .UseInMemoryDatabase("DbPersistenceHandlerTest")
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;

        context = new ErrorDbContext(inMemoryOption);
        context.Database.EnsureCreated();
    }

    [SetUp]
    public void Setup()
    {
        foreach (var item in context.Logs)
        {
            context.Logs.Remove(item);
        }

        context.SaveChanges();
    }

    [Test]
    public async Task StoreSingleException()
    {
        var repository = new Ef5Repository(() => new ErrorDbContext(inMemoryOption));
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
        var repository = new Ef5Repository(() => new ErrorDbContext(inMemoryOption));
        var handler = DbLogWriter.Create(repository, ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogWarnAsync(
            Icebear.Exceptions.Core.Tests.ExceptionProcessors.ExceptionUtilities.GetNestedException("Inner", "Outer"));

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
        var repository = new Ef5Repository(() => new ErrorDbContext(inMemoryOption));
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
        var repository = new Ef5Repository(() => new ErrorDbContext(inMemoryOption));
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
        var repository = new Ef5Repository(() => new ErrorDbContext(inMemoryOption));
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
    public async Task GetAllFilters_Date()
    {
        var repository = new Ef5Repository(() => new ErrorDbContext(inMemoryOption));
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
}