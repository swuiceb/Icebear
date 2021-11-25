using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Icebear.Exceptions.Db.Ef.LogWriters.RollingDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;

namespace Icebear.Exceptions.Db.Ef.Tests.LogWriters
{
    [TestFixture]
    public class ConcurrentListTests
    {
        private record TestItem
        {
            public string threadId { get; init; }
            public String name { get; init; }
            public Guid id { get; init; }
            public int index { get; init; }
        }

        [Test]
        public void WriteWhileFlush()
        {
            ConcurrentList<TestItem> concurrentList = new ();
            ConcurrentDictionary<int, ImmutableArray<TestItem>> combinedResults = new();
            
            var target = GetBaseData();
            var tasks = new List<Task>();
            
            for (int i = 0; i < 40; i++)
            {
                var threadId = i;
                var task = Task.Run(() =>
                {
                    var chunk = DoTest($"Thread: {threadId}");
                    combinedResults.AddOrUpdate(threadId, 
                        chunk,
                        (key, old) => chunk);
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            
            var totalLength = combinedResults.Sum(item => item.Value.Length);
            Assert.AreEqual(40, combinedResults.Count);
            Assert.AreEqual(40 * 4000, totalLength);
            

            ImmutableArray<TestItem> DoTest(String name)
            {
                foreach (var item in target)
                {
                    concurrentList.Add(new TestItem()
                    {
                        threadId = name,
                        id = item.id,
                        name = item.name,
                        index = item.index
                    });
                }

                return concurrentList.Flush();
            }
        }
        
        [Test]
        public void InsertToConcurrentList()
        {
            ConcurrentList<TestItem> concurrentList = new ();
            var target = GetBaseData();

            IList<Task> tasks = new List<Task>();
            Parallel.For(0, 30, i =>
            {
                var threadId = i;
                var task = Task.Run(() => DoTest($"Thread: {threadId}"));
                tasks.Add(task);
            });

            Task.WaitAll(tasks.ToArray());
            var results = concurrentList.Flush();
            
            Assert.AreEqual(30 * 4000, results.Length);
            for (int i = 0; i < 30; i++)
            {
                var threadId = i;
                var separatedResults = results.Where(r => r.threadId.Equals($"Thread: {threadId}"))
                    .ToList();
                Assert.AreEqual(4000, separatedResults.Count());
                // Assert.AreEqual(4000, separatedResults.DistinctBy(v => v.id).Count());
            }
            
            // Concurrently
            void DoTest(String name)
            {
                foreach (var item in target)
                {
                    concurrentList.Add(new TestItem()
                    {
                        threadId = name,
                        id = item.id,
                        name = item.name,
                        index = item.index
                    });
                }
            }
        }

        private static List<TestItem> GetBaseData()
        {
            var target = new List<TestItem>();
            for (int i = 0; i < 4000; i++)
            {
                target.Add(new TestItem()
                {
                    name = i.ToString(),
                    id = Guid.NewGuid(),
                    index = i
                });
            }

            return target;
        }
    }
}