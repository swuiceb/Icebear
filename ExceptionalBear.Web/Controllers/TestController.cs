using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExceptionalBear.Web.Views;
using Icebear.Exceptions.Core.LogReaders;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Microsoft.AspNetCore.Mvc;

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
        await exceptionLogWriter.LogErrorAsync(new Exception("My bad"));
        throw new Exception("My Bad");
    }

    [HttpGet("warning")]
    public async Task Warning()
    {
        await exceptionLogWriter.LogWarnAsync(new Exception("My bad"));
    }

    [HttpGet("log/{logtype}")]
    public async Task Log(LogType logType)
    {
        await exceptionLogWriter.LogAsync(logType, "custom", new
        {
            testString = "TestString",
            testInt = 3
        });
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
    public async Task<IActionResult> GetAll(String searchInput)
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
        else
        {
            ViewBag.Error = "Entry Code does not exist";
        }


        var results = await logReader.GetAll(new PageInfo()
        {
            Page = 0,
            PageSize = 10
        }, new FilterParam());

        var viewData = results.Items;

        ViewBag.LastN = viewData;
        return View("Index");
    }
}