using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Icebear.Exceptions.Db.Ef.LogWriters
{
    public sealed class DbLogWriter : DbLogWriterBase, ILogWriter
    {
        private readonly Func<ErrorDbContext> contextProvider;
        
        internal static DbLogWriter Create(
            [NotNull]Func<ErrorDbContext> contextProvider,
            Func<Exception, IErrorDescription>? exceptionTextProvider = null,
            Func<Exception, String>? sourceProvider = null,
            Func<Exception, String>? codeProvider = null)
        {
            return new DbLogWriter(
                    contextProvider, 
                    exceptionTextProvider,
                    sourceProvider, 
                    codeProvider)
                .Verify();
        }
        
        private DbLogWriter(
            [NotNull]Func<ErrorDbContext> contextProvider,
            Func<Exception, IErrorDescription>? exceptionTextProvider = null,
            Func<Exception, string>? sourceProvider = null,
            Func<Exception, string>? codeProvider = null) 
            :base(contextProvider, exceptionTextProvider, sourceProvider, codeProvider)
        {
            this.contextProvider = contextProvider;
        }

        /// <summary>
        /// Verify the dependencies are set
        /// </summary>
        private DbLogWriter Verify()
        {
            using var context = contextProvider.Invoke();
            
            // context.Database.Migrate();
            _ = context.Errors.Count();
            // if error occurs, an exception will be thrown
            return this;
        }
        
        public async Task<IError> LogErrorAsync(Exception exception)
        {
            return await StoreException(exception, LogType.Error);
        }

        public async Task<IError> LogWarnAsync(Exception exception)
        {
            return await StoreException(exception, LogType.Warning);
        }

        public async Task<string> LogAsync<T>(LogType logType, string message, T detail)
        {
            return await StoreLog(logType, message, detail);
        }
    }
}