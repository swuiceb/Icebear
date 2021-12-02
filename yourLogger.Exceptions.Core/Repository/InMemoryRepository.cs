using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Core.Repository
{
    public class InMemoryRepository : ILoggerRepository
    {
        private readonly Queue<ILogEntry> entries;
        private readonly IList<ITag> tags = new List<ITag>();
        private readonly int max;

        public InMemoryRepository(int max)
        {
            this.max = max;
            entries = new Queue<ILogEntry>(max);
        }

        public async Task<ILoggerRepository> VerifyAsync()
        {
            await Task.Delay(0);
            return this;
        }

        public async Task<IEnumerable<ITag>> GetTagsAsync()
        {
            await Task.Delay(0);
            return tags;
        }

        public async Task<IEnumerable<ILogEntry>> SaveAsync(IEnumerable<ILogEntry> logs)
        {
            await Task.Delay(0);
            while (entries.Count >= max)
            {
                entries.Dequeue();
            }

            foreach (var item in logs)
            {
                entries.Enqueue(item);
            }

            return logs.ToList();
        }

        public async Task<ILogEntry> SaveAsync(ILogEntry log)
        {
            return (await SaveAsync(new[] {log})).SingleOrDefault();
        }

        public Task<ILogEntry> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ILogEntry>> GetLastNEntriesAsync(int n, LogType[] applicableTypes)
        {
            throw new NotImplementedException();
        }

        public async Task<PageWrapper<ILogEntry>> GetAll(PageInfo pageInfo, FilterParam filters, SortByParam sortBy = null)
        {
            await Task.Delay(0);
            return new PageWrapper<ILogEntry>(RawList(), 0, 0, 0);
            
            IEnumerable<ILogEntry> RawList()
            {
                while (entries.Count > 0)
                {
                    yield return entries.Dequeue();
                }
            }
        }

        public Task UpdateUserContextAsync(Guid id, string userContext)
        {
            throw new NotImplementedException();
        }

        public void Flush(DateTimeOffset after)
        {
            entries.Clear();
        }

        public Task AddTagsAsync(IEnumerable<ITag> pendingTags)
        {
            foreach (var tag in pendingTags)
            {
                tags.Add(tag);
            }

            return Task.CompletedTask;
        }
    }
}