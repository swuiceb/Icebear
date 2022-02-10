using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.LogReaders;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Db.Ef.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using yourLogs.Exceptions.Core.Repository;

namespace yourLogs.Exceptions.Db.Ef.LogWriters
{
    public abstract class DbLogWriterBase : 
        LogWriterBase,
        ILogReader
    {
        private readonly ILoggerRepository repository;
        private IEnumerable<String> tags = Array.Empty<string>();

        protected DbLogWriterBase(
            [NotNull]ILoggerRepository repository,
            Func<Exception, ILogDescription>? exceptionTextProvider, 
            Func<Exception, string>? sourceProvider, 
            Func<Exception, string>? codeProvider,
            Func<String>? systemContextProvider
            ) : 
            base(exceptionTextProvider,
                sourceProvider, 
                codeProvider,
                systemContextProvider)
        {
            this.repository = repository;
            Init().Wait();
        }

        private async Task Init()
        {
            await repository.VerifyAsync();
            tags = (await repository.GetTagsAsync())
                .Select(t => t.Tag);
        }

        private async Task<String> TagsToRaw(IList<String> allRawTags)
        {
            var pendingTags = new List<TagEntity>();
            foreach (var tag in allRawTags)
            {
                if (!tags.Contains(tag))
                {
                    pendingTags.Add(new TagEntity(){Tag = tag});
                }
            }

            // if tag does not exist, need to insert tag
            await repository.AddTagsAsync(pendingTags);
            tags = tags.Concat(pendingTags.Select(p => p.Tag)).ToList();
            return String.Join(",", allRawTags);
        }

        protected async Task<ILogEntry> StoreLog<T>(LogType logType, string message, T detail, string[] tags)
        {
            var entity = Log2Entity(logType, detail, "", message, tags);

            await repository.SaveAsync(entity);

            return entity;
        }

        protected LogEntity Log2Entity<T>(LogType logType, T detail, string? code = null, string? text = null,
            params string[] tags)
        {
            return new LogEntity()
            {
                Id = Guid.NewGuid(),
                LogType = logType,
                Tags = TagsToRaw(tags).Result,
                Code = code,
                Text = text,
                SystemContext = SystemContextProvider?.Invoke(),
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
                Id = Guid.NewGuid(),
                LogType = logType,
                Code = CodeProvider?.Invoke(exception) ?? "",
                Source = SourceProvider?.Invoke(exception) ?? Environment.MachineName,
                Text = ExceptionTextProvider(exception).Text,
                SystemContext = SystemContextProvider?.Invoke(),
                Description = ExceptionTextProvider(exception).Description,
            };
        }

        protected async Task<ILogEntry> Store(ILogEntry entry)
        {
            return await repository.SaveAsync(entry);
        }
        protected async Task<ILogEntry> StoreException(Exception exception, LogType type, params String[] tags)
        {
            var entity = Exception2Log(exception, type);
            entity.Tags = await TagsToRaw(tags);
            await repository.SaveAsync(entity);

            return entity;
        }

        public async Task<IEnumerable<string>> GetTagsAsync()
        {
            await Task.Delay(0);
            return tags;
        }

        public async Task<ILogEntry> GetByIdAsync(Guid id)
        {
            return await repository.GetByIdAsync(id);
        }

        public virtual async Task<IEnumerable<ILogEntry>> GetLastNEntriesAsync(int n, LogType[] applicableTypes)
        {
            return await repository.GetLastNEntriesAsync(n, applicableTypes);
        }

        public virtual async Task<PageWrapper<ILogEntry>> GetAll(PageInfo pageInfo, FilterParam filters, SortByParam sortBy = null)
        {
            return await repository.GetAll(pageInfo, filters, sortBy);
        }

        public void Flush(DateTimeOffset after)
        {
            repository.Flush(after);
        }
    }
}