using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Core.LogReaders
{
    public interface ILogReader
    {
        Task<IEnumerable<string>> GetTagsAsync();
        Task<ILogEntry> GetByIdAsync(Guid id);
        Task<IEnumerable<ILogEntry>> GetLastNEntriesAsync(int n, LogType[] applicableTypes);
        Task<PageWrapper<ILogEntry>> GetAll(PageInfo pageInfo, FilterParam filters, SortByParam sortBy = null);
        void Flush(DateTimeOffset after);
    }
}