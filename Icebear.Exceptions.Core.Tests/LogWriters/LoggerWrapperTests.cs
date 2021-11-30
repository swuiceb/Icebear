using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.LogWriters.Providers;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Core.Tests.ExceptionProcessors;
using NSubstitute;
using NUnit.Framework;

namespace Icebear.Exceptions.Core.Tests.LogWriters
{
    [TestFixture]
    public class LoggerWrapperTests
    {
        [Test]
        public async Task LogWrapper_EnsureNoLogWhenLevelIsBelowTarget()
        {
            var logWriter = Substitute.For<ILogWriter>();
            var loggerWrapper = new LoggerWrapper(LogType.Error, logWriter);

            await loggerWrapper.LogWarnAsync(ExceptionUtilities.GetNestedException("inner", "outer"));
            
            await logWriter.DidNotReceiveWithAnyArgs().LogWarnAsync(Arg.Any<Exception>());
            await logWriter.DidNotReceiveWithAnyArgs().LogErrorAsync(Arg.Any<Exception>());
            await logWriter.DidNotReceiveWithAnyArgs() .LogAsync<dynamic>(Arg.Any<LogType>(), Arg.Any<string>(), Arg.Any<dynamic>());

            await loggerWrapper.LogErrorAsync(ExceptionUtilities.GetNestedException("inner", "outer"));
            await logWriter.Received(1).LogErrorAsync(Arg.Any<Exception>());
        }
        
        [Test]
        public async Task LogWrapper_EnsureParametersAreSubmitted()
        {
            var logWriter = Substitute.For<ILogWriter>();
            var loggerWrapper = new LoggerWrapper(LogType.Trace, logWriter);

            await loggerWrapper.LogErrorAsync(ExceptionUtilities.GetNestedException("inner", "outer"), "error");
            await loggerWrapper.LogWarnAsync(ExceptionUtilities.GetNestedException("inner", "outer"), "warn");
            await loggerWrapper.LogAsync(LogType.Custom, "Log", 5, "log", "custom");
            
            await logWriter.Received(1).LogErrorAsync(Arg.Any<Exception>(), "error");
            await logWriter.Received(1).LogWarnAsync(Arg.Any<Exception>(), "warn");
            await logWriter.Received(1).LogAsync(LogType.Custom, "Log", 5, "log", "custom");
        }
    }
}