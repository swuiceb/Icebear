using System;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionalBear.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogWriter exceptionLogWriter;

        public TestController(ILogWriter exceptionLogWriter)
        {
            this.exceptionLogWriter = exceptionLogWriter;
        }
        
        [HttpGet("exception")]
        public async Task Exception()
        {
            await exceptionLogWriter.LogErrorAsync(new Exception("My bad"));
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
    }
}