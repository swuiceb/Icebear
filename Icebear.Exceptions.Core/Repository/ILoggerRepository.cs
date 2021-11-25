using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;

public interface ILoggerRepository
{
    Task<IEnumerable<ILogEntry>> SaveAsync(IEnumerable<ILogEntry> logs);
    Task<ILogEntry> SaveAsync(ILogEntry log);

    Task<ILogEntry> GetByIdAsync(Guid id);
    Task<IEnumerable<ILogEntry>> GetLastNEntriesAsync(int n, LogType[] applicableTypes);
    Task<PageWrapper<ILogEntry>> GetAll(PageInfo pageInfo, FilterParam filters, SortByParam sortBy = null);
    
    void UpdateUserContext(Guid id, String userContext);
    void UpdateSystemContext(Guid id, String systemContext);
    
    void Flush(DateTimeOffset after);
}