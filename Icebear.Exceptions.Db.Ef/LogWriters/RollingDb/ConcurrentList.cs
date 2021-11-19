using System.Collections.Generic;
using System.Collections.Immutable;

namespace Icebear.Exceptions.Db.Ef.LogWriters.RollingDb
{
    public class ConcurrentList<T>
    {
        private static readonly object lockObject = new();
        private readonly List<T> list = new();

        public ImmutableArray<T> Flush()
        {
            lock (lockObject)
            {
                var result = ImmutableArray.Create<T>(list.ToArray());
                list.Clear();
                return result;
            }
        }

        public int Count()
        {
            lock (lockObject)
            {
                return list.Count;
            }
        }

        public void Add(T item)
        {
            lock (lockObject)
            {
                list.Add(item);
            }
        }

        public void Clear()
        {
            lock (lockObject)
            {
                list.Clear();
            }
        }
    }
}