using System;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.LogWriters.Providers;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Core.Tests.ExceptionProcessors;
using NUnit.Framework;

namespace Icebear.Exceptions.Core.Tests.PersistenceHandlers
{
    [TestFixture]
    public class LogWriterBuilderTests
    {
        [Test]
        public async Task DefaultOutOfBoxConfigInMemory()
        {
            // In memory, default exception text, uses machine name
            // empty code
            var inMemoryLogger = new LoggerBuilder()
                .BuildInMemory(3) as InMemoryLogWriter;

            Assert.IsNotNull(inMemoryLogger);
            await inMemoryLogger.LogErrorAsync(ExceptionUtilities.GetNestedException("inner", "outer"));
            
            var loggedItem = inMemoryLogger.GetAll().First();
            Assert.AreEqual(LogType.Error, loggedItem.LogType);
            Assert.IsFalse(loggedItem.Text.Contains("inner"));
            Assert.IsTrue(loggedItem.Text.Contains("outer"));
            
            Assert.IsTrue(loggedItem.Description.Contains("inner"));
            Assert.IsTrue(loggedItem.Description.Contains("outer"));
        }
        
        [Test]
        public async Task SimpleOutOfBoxConfigInMemory()
        {
            // In memory, default exception text, uses machine name
            // empty code
            var inMemoryLogger = new LoggerBuilder()
                .WithExceptionTextProvider(ExceptionTextProviders.Simple)
                .BuildInMemory(3) as InMemoryLogWriter;

            Assert.IsNotNull(inMemoryLogger);
            await inMemoryLogger.LogErrorAsync(new Exception("test"));
            
            var loggedItem = inMemoryLogger.GetAll().First();
            Assert.AreEqual(LogType.Error, loggedItem.LogType);
            Assert.AreEqual("test", loggedItem.Text.Trim());
            Assert.IsNull(loggedItem.Description);
        }
    }
}