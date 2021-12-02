using System;
using System.Diagnostics.CodeAnalysis;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Core.ExceptionProcessors;
using yourLogs.Exceptions.Core.LogWriters.Providers;

namespace yourLogs.Exceptions.Core.LogWriters
{
    public abstract class LogWriterBase
    {
        protected Func<Exception, string> SourceProvider { get; set; }
        protected Func<string> SystemContextProvider { get; private set; }
        protected Func<Exception, string> CodeProvider { get; set; }
        protected Func<Exception, ILogDescription> ExceptionTextProvider { get; set; }
        
        protected LogWriterBase(Func<Exception, ILogDescription> exceptionTextProvider = null,
            Func<Exception, string> sourceProvider = null,
            Func<Exception, string> codeProvider = null,
            Func<String> systemContextProvider = null)
        {
            ExceptionTextProvider = exceptionTextProvider ?? ExceptionTextProviders.Default;
            SourceProvider = sourceProvider;
            SystemContextProvider = systemContextProvider;
            CodeProvider = codeProvider;
        }
    }
}