using System;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.ExceptionProcessors;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;
using NUnit.Framework;

namespace yourLogs.Exceptions.Core.Tests.ExceptionProcessors
{
    [TestFixture]
    public class ExceptionProcessorTests
    {
        [Test]

        public void DefaultProcessor_NoLogger()
        {
            var defaultProcessor = ExceptionProcessorFactory.Default();
            defaultProcessor.WithLogger(null);
        }
        
        [Test]
        public async Task DefaultProcessor_InMemoryLogger()
        {
            var defaultProcessor = ExceptionProcessorFactory.Default();
            var logger = new InMemoryLogWriter();
            
            defaultProcessor.WithLogger(logger);
            await defaultProcessor.HandleException(new Exception("InMemoryException"));
            var loggedEntry = logger.GetAll().First();
            Assert.AreEqual(LogType.Error, loggedEntry.LogType);
            Assert.IsTrue(loggedEntry.Text.Contains("InMemoryException"));
        }

        [Test]
        public void DefaultProcessor_InMemoryException()
        {
            
        }
    }
}