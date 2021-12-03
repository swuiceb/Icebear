using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExceptionalBear.Web.Views;
using yourLogs.Exceptions.Core.LogReaders;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;
using Microsoft.AspNetCore.Mvc;

[Route("testV2")]
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

    [HttpPost("logs/all")]
    public async Task<IActionResult> Find(String searchInput)
    {
        if (Guid.TryParse(searchInput, out Guid id))
        {
            var logEntry = await logReader.GetByIdAsync(id);
            if (logEntry != null)
            {
                ViewBag.Specific = logEntry;
                return View("Index");
            }

            ViewBag.Error = "Entry Code does not exist";
        }

        return View("Index");
    }

    [HttpGet("logs/all")]
    public async Task<IActionResult> GetAll(
        String searchInput, 
        String[]logTypes,
        String[]tags,
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        if (Guid.TryParse(searchInput, out Guid id))
        {
            var logEntry = await logReader.GetByIdAsync(id);
            if (logEntry != null)
            {
                ViewBag.Specific = logEntry;
            }
            else
            {
                ViewBag.Error = "Entry Code does not exist";
            }
        }
        else if (searchInput != null)
        {
            ViewBag.Error = "Entry Code does not exist";
        }
   
        IEnumerable<LogType> typeFilters = new[] {LogType.Warning, LogType.Error, LogType.Custom};
        IEnumerable<String> allTags =  await logReader.GetTagsAsync();
        IEnumerable<String> selectedTags = 
            tags is {Length: > 0} ? tags : allTags.ToList();
        
        if (logTypes is {Length: > 0})
        {
            typeFilters = logTypes.Select(f => Enum.TryParse<LogType>(
                f,
                out LogType logType)
                ? logType
                : LogType.Error);
        }

        var results = await logReader.GetAll(new PageInfo()
        {
            Page = 0,
            PageSize = 10
        }, new FilterParam()
        {
            Since = from,
            Until = to,
            Tags = selectedTags,
            LogTypes = typeFilters.ToArray()
        });

        var viewData = results.Items;

        ViewBag.SelectedFilterTypes = typeFilters;
        
        ViewBag.Tags = allTags;
        ViewBag.SelectedTags = selectedTags;
        ViewBag.LastN = viewData;
        return View("Index");
    }
}