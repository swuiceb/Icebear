using System;
using System.Diagnostics;

namespace Icebear.Exceptions.Core.LogWriters.Providers
{
    public static class SystemContextProviders
    {
        public static Func<string> Default() => () =>
        {
            return $"[Machine]:{Environment.MachineName}," +
                   $"[Thread]: {Environment.CurrentManagedThreadId}," +
                   $"[CPU]: NOT IMPLEMENTED";
        };
    }
}