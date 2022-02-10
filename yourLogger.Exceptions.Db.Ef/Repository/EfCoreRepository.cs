using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Db.Ef.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace yourLogs.Exceptions.Db.Ef.Repository
{
    public class EfCoreRepository : EfCoreRepositoryBase
    {
        public EfCoreRepository(Func<ErrorDbContext> contextProvider) : base(contextProvider)
        {
        }
    }

    public class EfCoreInMemRepository : EfCoreRepositoryBase
    {
        public EfCoreInMemRepository(Func<ErrorDbContext> contextProvider) : base(contextProvider)
        {
        }

        public override Task<ILoggerRepository> VerifyAsync()
        {
            return VerifyForInMemAsync();
        }
    }
    
    public abstract class EfCoreRepositoryBase : ILoggerRepository
    {
        private readonly Func<ErrorDbContext> contextProvider;

        public EfCoreRepositoryBase([NotNull] Func<ErrorDbContext> contextProvider)
        {
            this.contextProvider = contextProvider;
        }

        public virtual async Task<ILoggerRepository> VerifyAsync()
        {
            var context = contextProvider();
            //await context.Database.EnsureCreatedAsync();
            await context.Database.MigrateAsync();
            return this;
        }

        protected async Task<ILoggerRepository> VerifyForInMemAsync()
        {
            var context = contextProvider();
            await context.Database.EnsureCreatedAsync();
            return this;
        }

        public async Task<IEnumerable<ITag>> GetTagsAsync()
        {
            await using var context = contextProvider();
            return context.Tags.ToList();
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
            
            await CloseConnection(context);
            return logEntries;
        }

        private static async Task CloseConnection(ErrorDbContext context)
        {
            if (context.Database.IsRelational())
            {
                await context.Database.CloseConnectionAsync();
            }
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
                queryable = queryable.Where(l => l.OccurredDate < filters.Until);
            }

            if (filters.Tags != null && filters.Tags.Any())
            {
                var expression = PredicateBuilder.False<LogEntity>();
                foreach (var tag in filters.Tags)
                {
                    Expression<Func<LogEntity, bool>> right = (l) => 
                        l.Tags != null &&
                        l.Tags.Contains(tag);
                    
                    expression = expression.Or(right);
                }

                queryable = queryable.Where(expression);
            }

            if (filters.LogTypes != null)
            {
                queryable = queryable.Where(l =>
                    filters.LogTypes.Length == 0 ||
                    filters.LogTypes.Contains(l.LogType));
            }

            if (sortBy == null)
            {
                queryable = queryable.OrderByDescending(q => q.OccurredDate);
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

        public async Task UpdateUserContextAsync(Guid id, string userContext)
        {
            await using var context = contextProvider();
            var log = (await context.Logs.FindAsync(id));
            await context.SaveChangesAsync();
        }

        public void Flush(DateTimeOffset after)
        {
            throw new NotImplementedException();
        }

        public async Task AddTagsAsync(IEnumerable<ITag> pendingTags)
        {
            await using var context = contextProvider();
            foreach (var tag in pendingTags)
            {
                await context.AddAsync((TagEntity) tag);
                await context.SaveChangesAsync();
            }

            await CloseConnection(context);
        }
    }
}