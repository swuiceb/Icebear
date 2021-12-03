using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.Models;
using IceBear.Exceptions.Core.Models.Entity;

namespace yourLogs.Exceptions.Core.LogWriters.Providers
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
            return await LogException(LogType.Error, exception, tags);
        }

        public async Task<ILogEntry> LogWarnAsync(Exception exception,params String[] tags)
        {
            return await LogException(LogType.Warning, exception, tags);
        }

        private async Task<ILogEntry> LogException(LogType level, Exception exception,params String[] tags)
        {
            if (logLevel <= level)
            {
                if (level == LogType.Error) 
                    return await logger.LogErrorAsync(exception, tags);
                
                if (level == LogType.Warning) 
                    return await logger.LogWarnAsync(exception, tags);
            }

            // TODO: Populate LogEntry with proper data
            return new LogModel();
        }

        public async Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail,params String[] tags)
        {
            if (logLevel <= logType)
            {
                return await logger.LogAsync(logType, message, detail, tags);
            }

            return null;
        }
    }
}