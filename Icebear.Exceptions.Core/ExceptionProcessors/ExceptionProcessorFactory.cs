using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Icebear.Exceptions.Core.ErrorMessageHandlers;

namespace Icebear.Exceptions.Core.ExceptionProcessors
{
    public static class ExceptionProcessorFactory
    {
        public static ExceptionProcessor Default()
        {
            return new ExceptionProcessor();
        }
    }
}