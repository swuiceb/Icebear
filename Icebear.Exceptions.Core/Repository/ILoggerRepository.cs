using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;

public interface ILoggerRepository
{
    Task<ILoggerRepository> VerifyAsync();
    
    Task<IEnumerable<ITag>> GetTagsAsync();
    Task<IEnumerable<ILogEntry>> SaveAsync(IEnumerable<ILogEntry> logs);
    Task<ILogEntry> SaveAsync(ILogEntry log);

    Task<ILogEntry> GetByIdAsync(Guid id);
    Task<IEnumerable<ILogEntry>> GetLastNEntriesAsync(int n, LogType[] applicableTypes);
    Task<PageWrapper<ILogEntry>> GetAll(PageInfo pageInfo, FilterParam filters, SortByParam sortBy = null);
    
    Task UpdateUserContextAsync(Guid id, String userContext);
    
    void Flush(DateTimeOffset after);
    Task AddTagsAsync(IEnumerable<ITag> pendingTags);
}