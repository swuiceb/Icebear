using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogReaders;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionalBear.Controllers
{
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
            return await logReader.GetLastNEntriesAsync(n, new []
            {
                LogType.Info,
                LogType.Custom,
                LogType.Error
            });
        }

        [HttpGet("logs/all")]
        public async Task<PageWrapper<ILogEntry>> GetAll()
        {
            return await logReader.GetAll(new PageInfo()
            {
                Page = 0,
                PageSize = 10
            }, new FilterParam());
        }
    }
}