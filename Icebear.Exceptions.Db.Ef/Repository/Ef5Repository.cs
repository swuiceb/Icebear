using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Db.Ef.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Icebear.Exceptions.Db.Ef.Repository
{
    public sealed class Ef5Repository : ILoggerRepository
    {
        private readonly Func<ErrorDbContext> contextProvider;

        public Ef5Repository([NotNull] Func<ErrorDbContext> contextProvider)
        {
            this.contextProvider = contextProvider;
        }

        public async Task<IEnumerable<ILogEntry>> SaveAsync(IEnumerable<ILogEntry> logs)
        {
            await using var context = contextProvider();
            var logEntries = logs as ILogEntry[] ?? logs.ToArray();

            foreach (var logEntry in logEntries)
            {
                var log = (LogEntity) logEntry;
                context.Logs.Add(log);
            }

            await context.SaveChangesAsync();
            
            if (context.Database.IsRelational())
            {
                await context.Database.CloseConnectionAsync();
            }
            return logEntries;
        }

        public async Task<ILogEntry> SaveAsync(ILogEntry log)
        {
            return (await SaveAsync(new[] {log})).Single();
        }

        public async Task<ILogEntry> GetByIdAsync(Guid id)
        {
            await using var context = contextProvider();
            return await context.Logs.FindAsync(id);
        }

        public async Task<IEnumerable<ILogEntry>> GetLastNEntriesAsync(int n, LogType[] applicableTypes)
        {
            await using var context = contextProvider?.Invoke();
            Debug.Assert(context != null, nameof(context) + " != null");

            return context.Logs.OrderByDescending(e => e.OccurredDate)
                .Take(n).AsEnumerable();
        }

        public async Task<PageWrapper<ILogEntry>> GetAll(PageInfo pageInfo, FilterParam filters, SortByParam sortBy = null)
        {
            await using var context = contextProvider?.Invoke();
            Debug.Assert(context != null, nameof(context) + " != null");

            var queryable = context.Logs.Where(l => true);

            if (filters.Since != null)
            {
                queryable = queryable.Where(l => l.OccurredDate > filters.Since);
            }

            if (filters.Until != null)
            {
                queryable = queryable.Where(l => l.OccurredDate < filters.Since);
            }

            if (filters.LogTypes != null)
            {
                queryable = queryable.Where(l =>
                    filters.LogTypes.Length == 0 ||
                    filters.LogTypes.Contains(l.LogType));
            }

            var total = await queryable.CountAsync();
            var logEntities = queryable.Skip(pageInfo.Page * pageInfo.PageSize)
                .Take(pageInfo.PageSize);

            return new PageWrapper<ILogEntry>(logEntities.AsEnumerable(),
                pageInfo.Page,
                total,
                pageInfo.PageSize
            );
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
            throw new NotImplementedException();
        }
    }
}