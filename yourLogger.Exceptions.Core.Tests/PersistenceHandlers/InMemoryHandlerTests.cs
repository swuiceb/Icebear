using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Core.LogWriters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace yourLogs.Exceptions.Core.Tests.PersistenceHandlers
{
    [TestFixture]
    public class InMemoryHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task StoreError_Verify_Error_Exists()
        {
            await StoreError_Verify_Exception_Exists(false);
        }
        
        [Test]
        public async Task StoreWarning_Verify_Warning_Exists()
        {
            await StoreError_Verify_Exception_Exists(true);
        }

        [Test]
        public async Task VerifyEntryRollOver()
        {
            var handler = new InMemoryLogWriter();
            handler.Initialize(2);
            await handler.LogErrorAsync(new Exception("First"));
            await handler.LogErrorAsync(new Exception("Second"));
            await handler.LogWarnAsync(new Exception("Third"));

            var results = handler.GetAll().ToList();
            Assert.AreEqual(2, results.Count);
            Assert.IsFalse(results.Any(ex=> ex.Text.Trim().Equals("First".Trim())));
            Assert.IsTrue(results.Any(ex=> ex.Text.Trim().Equals("Second".Trim())));
            Assert.IsTrue(results.Any(ex=> ex.Text.Trim().Equals("Third".Trim())));
        }

        [Test]
        public async Task VerifyCustomLogIsInOrder()
        {
            var handler = new InMemoryLogWriter();
            handler.Initialize(3);
            
            await handler.LogErrorAsync(new Exception("First"));
            await handler.LogAsync(LogType.Custom, "Custom log message", new {Value = 3});
            await handler.LogErrorAsync(new Exception("Second"));
            await handler.LogWarnAsync(new Exception("Third"));
            
            var results = handler.GetAll().ToList();
            var messageLog = results.ElementAt(0);
            var targetDetail = JsonConvert.DeserializeObject<KeyValuePair<string, int>>(messageLog.Description);
            
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(LogType.Custom, messageLog.LogType);
            Assert.AreEqual("Custom log message", messageLog.Text);
            Assert.AreEqual(3, targetDetail.Value);
        }

        [Test]
        public async Task VerifyCustomLogWithPrimitiveType()
        {
            var handler = new InMemoryLogWriter();
            
            await handler.LogAsync(LogType.Custom, "Custom log message", 3);
            
            var results = handler.GetAll().ToList();
            var messageLog = results.ElementAt(0);
            
            Assert.AreEqual("3", messageLog.Description.Trim());
        }
        
        private async Task StoreError_Verify_Exception_Exists(bool isWarning)
        {
            var handler = new InMemoryLogWriter();
            var type = isWarning ? LogType.Warning : LogType.Error;

            if (isWarning)
            {
                await handler.LogWarnAsync(new Exception("Exception Message"));
            }
            else
            {
                await handler.LogErrorAsync(new Exception("Exception Message"));
            }

            var results = handler.GetAll().ToList();
            var target = results.First();
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(type, target.LogType);
            Assert.AreEqual("Exception Message", target.Text.Trim());
        }
    }
}