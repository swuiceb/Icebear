using System;
using System.Diagnostics.CodeAnalysis;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Core.ExceptionProcessors;
using Icebear.Exceptions.Core.LogWriters.Providers;

namespace Icebear.Exceptions.Core.LogWriters
{
    public abstract class LogWriterBase
    {
        protected Func<Exception, string> SourceProvider { get; set; }
        protected Func<Exception, string> CodeProvider { get; set; }
        protected Func<Exception, IErrorDescription> ExceptionTextProvider { get; set; }
        
        protected LogWriterBase(
            [NotNull]Func<Exception, IErrorDescription> exceptionTextProvider = null,
            Func<Exception, string> sourceProvider = null,
            Func<Exception, string> codeProvider = null)
        {
            ExceptionTextProvider = exceptionTextProvider ?? ExceptionTextProviders.Default;
            this.SourceProvider = sourceProvider;
            this.CodeProvider = codeProvider;
        }
    }
}