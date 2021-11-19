using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.ExceptionProcessors;
using Icebear.Exceptions.Core.LogWriters.Providers;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Db.Ef.LogWriters;
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
                .Options;

        context = new ErrorDbContext(inMemoryOption);
        context.Database.EnsureCreated();
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task StoreSingleException()
    {
        var handler = DbLogWriter.Create(() =>
                new ErrorDbContext(inMemoryOption), ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogErrorAsync(new Exception("hi"));

        var result = await context.Errors.FirstAsync(e => e.Code.Equals("hi"));
        Assert.AreEqual(Environment.MachineName, result.Source);
        Assert.AreEqual(LogType.Error, result.LogType);
    }

    [Test]
    public async Task StoreNestedException()
    {
        var handler = DbLogWriter.Create(() =>
                new ErrorDbContext(inMemoryOption), ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogWarnAsync(
            Icebear.Exceptions.Core.Tests.ExceptionProcessors.ExceptionUtilities.GetNestedException("Inner", "Outer"));

        var result = await context.Errors.FirstAsync(e => e.Code.Equals("Outer"));
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
        var handler = DbLogWriter.Create(() =>
                new ErrorDbContext(inMemoryOption),
            ExceptionTextProviders.Default,
            sourceProvider: (ex) => "myTest",
            codeProvider: (ex) => ex.Message);

        await handler.LogErrorAsync(new Exception("SourceProvider"));

        var result = await context.Errors.FirstAsync(e => e.Code.Equals("SourceProvider"));
        Assert.AreEqual("myTest", result.Source);
        Assert.AreEqual(LogType.Error, result.LogType);
    }

    [Test]
    public async Task StoreCustomLog()
    {
        var handler = DbLogWriter.Create(() =>
                new ErrorDbContext(inMemoryOption), ExceptionTextProviders.Default,
            codeProvider: (ex) => ex.Message);

        await handler.LogAsync(LogType.Custom, "log message", new {Value = 3});
        var result = await context.Errors.FirstAsync(e => e.Text.Equals("log message"));
        Assert.AreEqual(Environment.MachineName, result.Source);
        Assert.AreEqual(LogType.Custom, result.LogType);

        var loggedObject = JsonConvert.DeserializeObject<KeyValuePair<string, int>>(result.Description);
        Assert.AreEqual(3, loggedObject.Value);
    }
}