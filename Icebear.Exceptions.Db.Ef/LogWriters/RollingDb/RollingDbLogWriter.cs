using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Icebear.Exceptions.Db.Ef.LogWriters.RollingDb
{
    public class RollingDbLogWriter : DbLogWriterBase, ILogWriter
    {
        private readonly int batchSize;
        private readonly ConcurrentList<LogEntity> rollingLog = new();
        
        internal RollingDbLogWriter(
            int batchSize,
            Func<ErrorDbContext> contextProvider, 
            Func<Exception, IErrorDescription>? exceptionTextProvider, 
            Func<Exception, string>? sourceProvider = null, 
            Func<Exception, string>? codeProvider = null) 
            : base(contextProvider, exceptionTextProvider, sourceProvider, codeProvider)
        {
            this.batchSize = batchSize;
        }
        
        // TODO: Use DbLogWriterBase, write N Logs into the db
        public async Task<IError> LogErrorAsync(Exception exception)
        {
            var error = Exception2Log(exception, LogType.Error);
            rollingLog.Add(error);
            // TODO: Once rollingLog hits maximum. Store

            if (rollingLog.Count() >= batchSize)
            {
                // Atomic
                ImmutableArray<LogEntity> immutableList = rollingLog.Flush();
                await StoreInDb(immutableList);
            }
            return error;
        }

        public async Task<IError> LogWarnAsync(Exception exception)
        {
            var error = Exception2Log(exception, LogType.Warning);
            rollingLog.Add(error);
            
            if (rollingLog.Count() >= batchSize)
            {
                // Atomic
                ImmutableArray<LogEntity> immutableList = rollingLog.Flush();
                await StoreInDb(immutableList);
            }
            
            return error;
        }

        public async Task<string> LogAsync<T>(LogType logType, string message, T detail)
        {
            var log = Log2Entity(logType, detail, text: message);
            rollingLog.Add(log);
            
            if (rollingLog.Count() >= batchSize)
            {
                // Atomic
                ImmutableArray<LogEntity> immutableList = rollingLog.Flush();
                await StoreInDb(immutableList);
            }
            
            return "";
        }

        private async Task StoreInDb(ImmutableArray<LogEntity> logs)
        {
            await using var context = ContextProvider();
            
            foreach (var entity in logs)
            {
                context.Errors.Add(entity);
            }

            await context.SaveChangesAsync();
            //await context.Database.CloseConnectionAsync();
        }
    }
}