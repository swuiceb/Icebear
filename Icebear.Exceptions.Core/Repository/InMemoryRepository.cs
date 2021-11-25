using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.Repository
{
    public class InMemoryRepository : ILoggerRepository
    {
        private readonly Queue<ILogEntry> entries;
        private readonly int max;

        public InMemoryRepository(int max)
        {
            this.max = max;
            entries = new Queue<ILogEntry>(max);
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

        public void UpdateUserContext(Guid id, string userContext)
        {
            throw new NotImplementedException();
        }

        public void UpdateSystemContext(Guid id, string systemContext)
        {
            throw new NotImplementedException();
        }

        public void Flush(DateTimeOffset after)
        {
            entries.Clear();
        }
    }
}