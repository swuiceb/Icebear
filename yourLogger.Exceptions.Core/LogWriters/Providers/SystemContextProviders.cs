using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace yourLogs.Exceptions.Core.LogWriters.Providers
{
    public static class SystemContextProviders
    {
        public static Func<string> Default() => () =>
        {
            return $"[Machine]:{Environment.MachineName}," +
                   $"[Thread]: {Environment.CurrentManagedThreadId}";
        };
    }
}