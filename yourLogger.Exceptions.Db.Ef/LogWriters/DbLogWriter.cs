using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Db.Ef.LogWriters
{
    public sealed class DbLogWriter : DbLogWriterBase, ILogWriter
    {
        internal static DbLogWriter Create(
            [NotNull]ILoggerRepository repository,
            Func<Exception, ILogDescription>? exceptionTextProvider = null,
            Func<Exception, String>? sourceProvider = null,
            Func<Exception, String>? codeProvider = null,
            Func<String>? systemContextProvider = null)
        {
            return new DbLogWriter(
                    repository, 
                    exceptionTextProvider,
                    sourceProvider, 
                    codeProvider,
                    systemContextProvider)
                .Verify();
        }
        
        private DbLogWriter(
            [NotNull]ILoggerRepository repository,
            Func<Exception, ILogDescription>? exceptionTextProvider = null,
            Func<Exception, string>? sourceProvider = null,
            Func<Exception, string>? codeProvider = null,
            Func<string>? systemContextProvider = null) 
            :base(repository, exceptionTextProvider, sourceProvider, codeProvider, systemContextProvider)
        {
        }

        /// <summary>
        /// Verify the dependencies are set
        /// </summary>
        private DbLogWriter Verify()
        {
            // using var context = contextProvider.Invoke();
            
            // context.Database.Migrate();
            // _ = context.Logs.Count();
            // if error occurs, an exception will be thrown
            return this;
        }

        public ILogEntry LogError(Exception exception, params string[] tags)
        {
            return LogErrorAsync(exception, tags).Result;
        }

        public ILogEntry LogWarn(Exception exception, params string[] tags)
        {
            return LogWarnAsync(exception, tags).Result;
        }

        public ILogEntry Log<T>(LogType logType, string message, T detail, params string[] tags)
        {
            return LogAsync(logType, message, detail, tags).Result;
        }

        public async Task<ILogEntry> LogErrorAsync(Exception exception,params String[] tags)
        {
            return await StoreException(exception, LogType.Error,tags);
        }

        public async Task<ILogEntry> LogWarnAsync(Exception exception,params String[] tags)
        {
            return await StoreException(exception, LogType.Warning, tags);
        }

        public async Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail,params String[] tags)
        {
            return await StoreLog(logType, message, detail, tags);
        }
    }
}