using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.ExceptionProcessors;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Core.Models.Entity;
using Newtonsoft.Json;

namespace Icebear.Exceptions.Core.LogWriters
{
    internal class InMemoryLogWriter : LogWriterBase, ILogWriter
    {
        private Queue<LogEntity> entries = new Queue<LogEntity>(100);
        private int max = 100;
        
        private InMemoryLogWriter(){}
        
        public InMemoryLogWriter(
            Func<Exception, IErrorDescription> exceptionTextProvider = null,
            Func<Exception, string> sourceProvider = null,
            Func<Exception, string> codeProvider = null) 
            :base(exceptionTextProvider, sourceProvider, codeProvider)
        {
        }

        public InMemoryLogWriter Initialize(int max)
        {
            this.max = max;
            entries = new Queue<LogEntity>();
            return this;
        }

        public async Task<IError> LogErrorAsync(Exception exception)
        {
            var error = await Task.Run(() => 
                Enqueue(LogType.Error, exception));
            return error;
        }

        public async Task<IError> LogWarnAsync(Exception exception)
        {
            return await Task.Run(() => Enqueue(LogType.Warning, exception));
        }

        public async Task<string> LogAsync<T>(LogType logType, string message, T detail)
        {
            await Task.Run(() => EnqueueLog(logType, message, detail));
            return entries.Count.ToString();
        }

        internal IEnumerable<LogEntity> GetAll()
        {
            while(entries.Count > 0)
            {
                yield return entries.Dequeue();
            } 
        }

        private LogEntity Enqueue(LogType type, Exception exception)
        {
            while (entries.Count >= max)
            {
                entries.Dequeue();
            }

            var target = ToInMemoryException(type, exception);
            entries.Enqueue(target);
            return target;
        }

        private LogEntity EnqueueLog<T>(LogType type, string message, T data)
        {
            while (entries.Count >= max)
            {
                entries.Dequeue();
            }

            if ((type == LogType.Error || type == LogType.Warning) &&
                typeof(T).IsAssignableTo(typeof(Exception)))
            {
                return Enqueue(type, data as Exception);
            }
            else
            {
                var target = ToInMemoryEntry(type, data, text: message);
                entries.Enqueue(target);
                return target;
            }
        }
        
        private LogEntity ToInMemoryException(LogType logType, Exception exception)
        {
            return new LogEntity()
            {
                Code = CodeProvider?.Invoke(exception) ?? "",
                LogType = logType,
                Text = ExceptionTextProvider(exception).Text,
                Description = ExceptionTextProvider(exception).Description,
            };
        }

        private LogEntity ToInMemoryEntry<T>(LogType logType, T detail, string code = null, string text = null)
        {
            return new LogEntity()
            {
                LogType = logType,
                Code = code,
                Text = text,
                Source = SourceProvider?.Invoke(null) ?? Environment.MachineName,
                Description = 
                    (typeof(T) != typeof(string) && !typeof(T).IsValueType) ? 
                        JsonConvert.SerializeObject(detail) : detail.ToString()
            };
        }
    }
}