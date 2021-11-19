using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Core.Models.Entity;
using Newtonsoft.Json;

namespace Icebear.Exceptions.Db.Ef.LogWriters
{
    public abstract class DbLogWriterBase : LogWriterBase
    {
        protected readonly Func<ErrorDbContext> ContextProvider;

        protected DbLogWriterBase(
            [NotNull]Func<ErrorDbContext> contextProvider,
            Func<Exception, IErrorDescription>? exceptionTextProvider, 
            Func<Exception, string>? sourceProvider, 
            Func<Exception, string>? codeProvider) : 
            base(exceptionTextProvider,
                sourceProvider, 
                codeProvider)
        {
            this.ContextProvider = contextProvider;
        }

        protected async Task<string> StoreLog<T>(LogType logType, string message, T detail)
        {
            await using var context = ContextProvider?.Invoke();
            Debug.Assert(context != null, nameof(context) + " != null");
            
            var entity = new LogEntity()
            {
                Id = Guid.NewGuid().ToString(),
                LogType = logType,
                Code =  "",
                Source = Environment.MachineName,
                Text = message,
                Description = (typeof(T) != typeof(string) && !typeof(T).IsValueType) ? 
                    JsonConvert.SerializeObject(detail) : detail?.ToString()
            };

            context.Errors.Add(entity);
            await context.SaveChangesAsync();

            return entity.Id;
        }

        protected LogEntity Log2Entity<T>(LogType logType, T detail, string? code = null, string? text = null)
        {
            return new LogEntity()
            {
                LogType = logType,
                Code = code,
                Text = text,
                Source = SourceProvider?.Invoke(null) ?? Environment.MachineName,
                Description = 
                    (typeof(T) != typeof(string) && !typeof(T).IsValueType) ? 
                        JsonConvert.SerializeObject(detail) : detail?.ToString()
            };
        }
        protected LogEntity Exception2Log(Exception exception, LogType logType)
        {

            return new LogEntity()
            {
                Id = Guid.NewGuid().ToString(),
                LogType = logType,
                Code = CodeProvider?.Invoke(exception) ?? "",
                Source = SourceProvider?.Invoke(exception) ?? Environment.MachineName,
                Text = ExceptionTextProvider(exception).Text,
                Description = ExceptionTextProvider(exception).Description,
            };
        }

        protected async Task<IError> StoreException(Exception exception, LogType type)
        {
            await using var context = ContextProvider?.Invoke();
            Debug.Assert(context != null, nameof(context) + " != null");
            
            var entity = Exception2Log(exception, type);

            context.Errors.Add(entity);
            await context.SaveChangesAsync();

            return entity;
        } 
    }
}