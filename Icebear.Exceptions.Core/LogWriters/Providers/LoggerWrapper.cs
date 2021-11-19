using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.LogWriters.Providers
{
    internal class LoggerWrapper : ILogWriter
    {
        private readonly LogType logLevel;
        private readonly ILogWriter logger;

        internal LoggerWrapper(LogType logLevel, ILogWriter logger)
        {
            this.logLevel = logLevel;
            this.logger = logger;
        }

        public async Task<IError> LogErrorAsync(Exception exception)
        {
            await LogException(LogType.Error, exception);
            return null;
        }

        public async Task<IError> LogWarnAsync(Exception exception)
        {
            await LogException(LogType.Warning, exception);
            return null;
        }

        private async Task LogException(LogType level, Exception exception)
        {
            if (logLevel <= level)
            {
                if (logLevel == LogType.Error) 
                    await logger.LogErrorAsync(exception);
                
                if (logLevel == LogType.Warning) 
                    await logger.LogWarnAsync(exception);
            }
        }

        public async Task<string> LogAsync<T>(LogType logType, string message, T detail)
        {
            if (logLevel >= logType)
            {
                await logger.LogAsync(logType, message, detail);
            }

            return null;
        }
    }
}