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

        public async Task<ILogEntry> LogErrorAsync(Exception exception,params String[] tags)
        {
            await LogException(LogType.Error, exception);
            return null;
        }

        public async Task<ILogEntry> LogWarnAsync(Exception exception,params String[] tags)
        {
            await LogException(LogType.Warning, exception, tags);
            return null;
        }

        private async Task LogException(LogType level, Exception exception,params String[] tags)
        {
            if (logLevel <= level)
            {
                if (logLevel == LogType.Error) 
                    await logger.LogErrorAsync(exception, tags);
                
                if (logLevel == LogType.Warning) 
                    await logger.LogWarnAsync(exception, tags);
            }
        }

        public async Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail,params String[] tags)
        {
            if (logLevel >= logType)
            {
                return await logger.LogAsync(logType, message, detail, tags);
            }

            return null;
        }
    }
}