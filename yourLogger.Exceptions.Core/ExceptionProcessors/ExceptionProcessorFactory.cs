using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using yourLogs.Exceptions.Core.ErrorMessageHandlers;

namespace yourLogs.Exceptions.Core.ExceptionProcessors
{
    public static class ExceptionProcessorFactory
    {
        public static ExceptionProcessor Default()
        {
            return new ExceptionProcessor();
        }
    }
}