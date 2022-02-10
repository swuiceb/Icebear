using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Core.Repository;
using yourLogs.Exceptions.Db.Ef.Models;

namespace yourLogs.Exceptions.Db.Ef.LogWriters.RollingDb
{
    public class RollingDbLogWriter : DbLogWriterBase, ILogWriter
    {
        private readonly int batchSize;
        private readonly ConcurrentList<LogEntity> rollingLog = new();
        
        internal RollingDbLogWriter(
            int batchSize,
            [NotNull]ILoggerRepository repository, 
            Func<Exception, ILogDescription>? exceptionTextProvider, 
            Func<Exception, string>? sourceProvider = null, 
            Func<Exception, string>? codeProvider = null,
            Func<string>? systemContextProvider = null) 
            : base(repository, exceptionTextProvider, sourceProvider, codeProvider, systemContextProvider)
        {
            this.batchSize = batchSize;
        }
        
        // TODO: Use DbLogWriterBase, write N Logs into the db
        public ILogEntry LogError(Exception exception, params string[] tags)
        {
            // TODO: make sync version of storage, and flush
            throw new NotImplementedException();
        }

        public ILogEntry LogWarn(Exception exception, params string[] tags)
        {
            // TODO: make sync version of storage, and flush
            throw new NotImplementedException();
        }

        public ILogEntry Log<T>(LogType logType, string message, T detail, params string[] tags)
        {
            // TODO: make sync version of storage, and flush
            throw new NotImplementedException();
        }

        public async Task<ILogEntry> LogErrorAsync(Exception exception,params String[] tags)
        {
            var error = Exception2Log(exception, LogType.Error);
            rollingLog.Add(error);
            // TODO: Once rollingLog hits maximum. Store

            if (rollingLog.Count() >= batchSize)
            {
                // Atomic
                ImmutableArray<LogEntity> immutableList = rollingLog.Flush();
                await StoreInDbAsync(immutableList);
            }
            return error;
        }

        public async Task<ILogEntry> LogWarnAsync(Exception exception,params String[] tags)
        {
            var error = Exception2Log(exception, LogType.Warning);
            rollingLog.Add(error);
            
            if (rollingLog.Count() >= batchSize)
            {
                // Atomic
                ImmutableArray<LogEntity> immutableList = rollingLog.Flush();
                await StoreInDbAsync(immutableList);
            }
            
            return error;
        }

        public async Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail,params String[] tags)
        {
            var log = Log2Entity(logType, detail, code: "", text: message, tags);
            rollingLog.Add(log);
            
            if (rollingLog.Count() >= batchSize)
            {
                // Atomic
                ImmutableArray<LogEntity> immutableList = rollingLog.Flush();
                await StoreInDbAsync(immutableList);
            }
            
            return log;
        }

        public override async Task<IEnumerable<ILogEntry>> GetLastNEntriesAsync(int n, LogType[] applicableTypes)
        {
            await StoreInDbAsync(rollingLog.Flush());
            return await base.GetLastNEntriesAsync(n, applicableTypes);
        }

        public override async Task<PageWrapper<ILogEntry>> GetAll(PageInfo pageInfo, FilterParam filters, SortByParam sortBy = null)
        {
            await StoreInDbAsync(rollingLog.Flush());
            return await base.GetAll(pageInfo, filters, sortBy);
        }

        private async Task StoreInDbAsync(ImmutableArray<LogEntity> logs)
        {
            if (logs.IsEmpty) return;
            foreach (var entity in logs)
            {
                await Store(entity);
            }
        }
    }
}