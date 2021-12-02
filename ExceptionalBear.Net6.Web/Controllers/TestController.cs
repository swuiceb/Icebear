using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using yourLogs.Exceptions.Core.LogReaders;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;

public class TestController : Controller
{
    private readonly ILogWriter exceptionLogWriter;
    private readonly ILogReader logReader;

    public TestController(
        ILogWriter exceptionLogWriter,
        ILogReader logReader)
    {
        this.exceptionLogWriter = exceptionLogWriter;
        this.logReader = logReader;
    }

    [HttpGet("exception")]
    public async Task Exception()
    {
        await exceptionLogWriter.LogErrorAsync(new Exception("My bad"), "myTest", "exception");
        throw new Exception("My Bad");
    }

    [HttpGet("warning")]
    public async Task Warning()
    {
        await exceptionLogWriter.LogWarnAsync(new Exception("My bad"), "warn");
    }

    [HttpGet("log/{logtype}")]
    public async Task Log(LogType logType)
    {
        await exceptionLogWriter.LogAsync(logType, "custom", new
        {
            testString = "TestString",
            testInt = 3
        }, "log", "mytest");
    }

    [HttpGet("logs/lastN/{n}")]
    public async Task<IEnumerable<ILogEntry>> Filter(int n)
    {
        return await logReader.GetLastNEntriesAsync(n, new[]
        {
            LogType.Info,
            LogType.Custom,
            LogType.Error
        });
    }
}