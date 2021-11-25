using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;
using IceBear.Exceptions.Core.Models.Entity;
using Icebear.Exceptions.Core.Repository;
using Newtonsoft.Json;

namespace Icebear.Exceptions.Core.LogWriters
{
    internal class InMemoryLogWriter : LogWriterBase, ILogWriter
    {
        private ILoggerRepository repository;

        private InMemoryLogWriter()
        {
        }

        public InMemoryLogWriter(
            Func<Exception, ILogDescription> exceptionTextProvider = null,
            Func<Exception, string> sourceProvider = null,
            Func<Exception, string> codeProvider = null)
            : base(exceptionTextProvider, sourceProvider, codeProvider)
        {
            this.repository = new InMemoryRepository(100);
        }

        public InMemoryLogWriter Initialize(int max)
        {
            this.repository = new InMemoryRepository(max);
            return this;
        }

        public async Task<ILogEntry> LogErrorAsync(Exception exception,params String[] tags)
        {
            var entry = ToInMemoryException(LogType.Error, exception);
            var error = await repository.SaveAsync(entry);
            return error;
        }

        public async Task<ILogEntry> LogWarnAsync(Exception exception,params String[] tags)
        {
            var entry = ToInMemoryException(LogType.Warning, exception);
            return await repository.SaveAsync(entry);
        }

        public async Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail,params String[] tags)
        {
            var entry = ToInMemoryEntry(logType, detail, "", message);
            return await repository.SaveAsync(entry);
        }

        internal IEnumerable<ILogEntry> GetAll()
        {
            return repository.GetAll(null, new FilterParam()).Result.Items;
        }
        
        private ILogEntry ToInMemoryException(LogType logType, Exception exception)
        {
            return new LogModel()
            {
                Id = Guid.NewGuid(),
                Code = CodeProvider?.Invoke(exception) ?? "",
                LogType = logType,
                Text = ExceptionTextProvider(exception).Text,
                Description = ExceptionTextProvider(exception).Description,
            };
        }

        private LogModel ToInMemoryEntry<T>(LogType logType, T detail, string code = null, string text = null)
        {
            return new LogModel()
            {
                LogType = logType,
                Code = code,
                Text = text,
                Source = SourceProvider?.Invoke(null) ?? Environment.MachineName,
                Description =
                    (typeof(T) != typeof(string) && !IsUnSerializableType(typeof(T)))
                        ? JsonConvert.SerializeObject(detail)
                        : detail.ToString()
            };
        }

        private bool IsUnSerializableType(Type t)
        {
            return t == typeof(int) ||
                   t == typeof(long) ||
                   t == typeof(String) ||
                   t == typeof(DateTime) ||
                   t == typeof(float) ||
                   t == typeof(decimal) ||
                   t == typeof(double) ||
                   t == typeof(Single);
        }
    }
}