using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogReaders;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Db.Ef.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Icebear.Exceptions.Db.Ef.LogWriters
{
    public abstract class DbLogWriterBase : 
        LogWriterBase,
        ILogReader
    {
        private readonly ILoggerRepository repository;
        private IEnumerable<String> tags = new String[] { };

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

        public async Task Init()
        {
            tags = (await repository.GetTagsAsync())
                .ToList()
                .Select(t => t.Tag);
        }

        protected async Task<String> TagsToRaw(IEnumerable<String> tags)
        {
            var pendingTags = new List<TagEntity>();
            foreach (var tag in tags)
            {
                if (!this.tags.Contains(tag))
                {
                    pendingTags.Add(new TagEntity(){Tag = tag});
                }
            }

            // if tag does not exist, need to insert tag
            await repository.AddTagsAsync(pendingTags);
            this.tags = this.tags.Concat(pendingTags.Select(p => p.Tag)).ToList();
            return String.Join(",", tags);
        }

        protected async Task<ILogEntry> StoreLog<T>(LogType logType, string message, T detail, string[] tags)
        {
            var entity = new LogEntity()
            {
                Id = Guid.NewGuid(),
                LogType = logType,
                Tags = await TagsToRaw(tags),
                Code =  "",
                Source = Environment.MachineName,
                Text = message,
                Description = (typeof(T) != typeof(string) && !typeof(T).IsValueType) ? 
                    JsonConvert.SerializeObject(detail) : detail?.ToString()
            };

            await repository.SaveAsync(entity);

            return entity;
        }

        protected LogEntity Log2Entity<T>(LogType logType, T detail, string? code = null, string? text = null)
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