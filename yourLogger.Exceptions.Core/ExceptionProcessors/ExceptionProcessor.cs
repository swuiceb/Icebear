using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.ErrorMessageHandlers;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Core.ExceptionProcessors
{
    public class ExceptionProcessor : IExceptionProcessor
    {
        private IEnumerable<IExceptionPreProcessor> exceptionTransformers = Enumerable.Empty<IExceptionPreProcessor>();
        private ILogWriter logger = new InMemoryLogWriter();
        
        internal ExceptionProcessor()
        {
        }

        public void RegisterPreHandlers(IEnumerable<IExceptionPreProcessor> transformers)
        {
            exceptionTransformers = exceptionTransformers.Union(transformers.ToList());
        }

        public void RegisterPreHandlers(params IExceptionPreProcessor[] transformers)
        {
            RegisterPreHandlers(transformers.AsEnumerable());
        }

        public IExceptionProcessor WithLogger(ILogWriter _logger)
        {
            this.logger = _logger;
            return this;
        }

        public async Task<ILogEntry> HandleException(Exception ex)
        {
            var transformedException = ex;
            
            foreach (var transformer in exceptionTransformers)
            {
                transformedException = DoProcessException(transformer, transformedException);
            }

            return await logger?.LogErrorAsync(transformedException);
            
            Exception DoProcessException(IExceptionPreProcessor transformer, Exception targetException)
            {
                return transformer.PreProcessException(targetException);
            }
        }
    }
}