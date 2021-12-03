using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace yourLogs.Exceptions.Core.Models
{
    public sealed class PageWrapper<T> : PageInfo
    {
        public PageWrapper(IEnumerable<T> items, int start, int total, int pageSize)
        {
            this.Page = start;
            this.Total = total;
            PageSize = pageSize;
            Items = items.ToList();
        }
        public IEnumerable<T> Items { get; set; }
    }
}