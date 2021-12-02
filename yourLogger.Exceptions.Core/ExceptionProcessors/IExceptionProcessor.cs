using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.ErrorMessageHandlers;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Core.ExceptionProcessors
{
    public interface IExceptionProcessor
    {
        void RegisterPreHandlers(IEnumerable<IExceptionPreProcessor> transformers);
        void RegisterPreHandlers(params IExceptionPreProcessor[] transformers);

        Task<ILogEntry> HandleException(Exception ex);
        
        IExceptionProcessor WithLogger(ILogWriter logger);
    }
}