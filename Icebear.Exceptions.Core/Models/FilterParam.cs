using System;
using System.Collections.Generic;

namespace Icebear.Exceptions.Core.Models
{
    public class FilterParam
    {
        public IEnumerable<String> Tags { get; set; }
        public bool ContainUserContext { get; set; }
        public LogType[] LogTypes { get; set; }
        public DateTimeOffset? Since { get; set; }
        public DateTimeOffset? Until { get; set; }
    }
}