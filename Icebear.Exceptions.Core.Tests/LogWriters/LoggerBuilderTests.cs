using System.Linq;
using System.Reflection;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.LogWriters.Providers;
using Icebear.Exceptions.Core.Models;
using NUnit.Framework;

namespace Icebear.Exceptions.Core.Tests.LogWriters
{
    [TestFixture]
    public class LoggerBuilderTests
    {
        [Test]
        public void Build_WithoutWriter()
        {
            var builder = new LoggerBuilder();
            var logger = builder.Build(LogType.Info).Writer;

            Assert.IsInstanceOf<LoggerWrapper>(logger);
            var wrapper = (LoggerWrapper) logger;
            var fieldInfos = typeof(LoggerWrapper).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            var actualLogger = fieldInfos.First(f => f.FieldType == typeof(ILogWriter)).GetValue(wrapper);
            
            Assert.IsInstanceOf<ConsoleLogWriter>(actualLogger);
        }

        [Test]
        public void Build_WithWriter()
        {
            var builder = new LoggerBuilder();
            var inMemoryLogger = builder.BuildInMemory(30);
            
            var logger = builder
                .WithWriter(inMemoryLogger)
                .Build(LogType.Info);

            Assert.IsInstanceOf<LoggerWrapper>(logger.Writer);
            var wrapper = (LoggerWrapper) logger.Writer;
            var fieldInfos = typeof(LoggerWrapper).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            var targetFieldInfo = fieldInfos.First(f => f.FieldType == typeof(ILogWriter));
            var actualLogger = targetFieldInfo.GetValue(wrapper);
            
            Assert.IsInstanceOf<ConsoleLogWriter>(actualLogger);
            var consoleLogWriter = ((ConsoleLogWriter) actualLogger);
            
            var consoleFields = typeof(ConsoleLogWriter).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var targetConsoleFieldInfo = consoleFields.First(f => f.FieldType == typeof(ILogWriter));
            
            Assert.IsInstanceOf<InMemoryLogWriter>(targetConsoleFieldInfo.GetValue(consoleLogWriter));
        }

        [Test]
        public void BuildWithoutConsole()
        {
            var builder = new LoggerBuilder();
            var inMemoryLogger = builder.BuildInMemory(30);
            
            var logger = builder
                .DisableConsole()
                .WithWriter(inMemoryLogger)
                .Build(LogType.Info).Writer;

            Assert.IsInstanceOf<LoggerWrapper>(logger);
            var wrapper = (LoggerWrapper) logger;
            var fieldInfos = typeof(LoggerWrapper).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            var targetFieldInfo = fieldInfos.First(f => f.FieldType == typeof(ILogWriter));
            var actualLogger = targetFieldInfo.GetValue(wrapper);
            
            Assert.IsNotInstanceOf<ConsoleLogWriter>(actualLogger); 
            Assert.IsInstanceOf<InMemoryLogWriter>(actualLogger); 
        }
    }
}