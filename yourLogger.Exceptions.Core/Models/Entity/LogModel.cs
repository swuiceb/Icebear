using System;
using yourLogs.Exceptions.Core.Models;

namespace IceBear.Exceptions.Core.Models.Entity
{
    public class LogModel : ILogEntry
    {
        public Guid Id { get; internal set; }
        public string Code { get; internal set; }
        public string Tags { get; }


        public DateTimeOffset OccurredDate { get; internal set; } = DateTimeOffset.Now;

        public LogType LogType { get; set; }

        public string Source { get; internal set; }
        public string Text { get; internal set; }

        public string Description { get; internal set; }
        public string UserContext { get; }
        public string SystemContext { get; internal set; }
    }
}