using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.ErrorMessageHandlers;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.ExceptionProcessors
{
    public interface IExceptionProcessor
    {
        void RegisterPreHandlers(IEnumerable<IExceptionPreProcessor> transformers);
        void RegisterPreHandlers(params IExceptionPreProcessor[] transformers);

        Task<ILogEntry> HandleException(Exception ex);
        
        IExceptionProcessor WithLogger(ILogWriter logger);
    }
}