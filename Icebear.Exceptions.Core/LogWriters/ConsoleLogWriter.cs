using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.LogWriters
{
    internal class ConsoleLogWriter : LogWriterBase, ILogWriter
    {
        private readonly ILogWriter writer;
        private readonly string format;

        internal ConsoleLogWriter([NotNull]ILogWriter writer,
            string format,
            Func<Exception, IErrorDescription> exceptionTextProvider,
            Func<Exception, string> codeTextProvider,
            Func<Exception, string> sourceProvider) 
            : base(exceptionTextProvider, codeTextProvider, sourceProvider)
        {
            this.writer = writer;
            this.format = format;
        }

        public async Task<IError> LogErrorAsync(Exception exception)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            // TODO: if a format is given, respect the format
            Console.WriteLine($"{DateTime.Now}:[{threadId}][ERROR]: {exception}");
            await writer.LogErrorAsync(exception);
            return null;
        }

        public async Task<IError> LogWarnAsync(Exception exception)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{DateTime.Now}:[{threadId}][WARN]: {exception}");
            await writer.LogWarnAsync(exception);
            return null;
        }

        public async Task<string> LogAsync<T>(LogType logType, string message, T detail)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{DateTime.Now}:[{threadId}][{logType.ToString().ToUpper()}]: {message}");
            Console.WriteLine($"{DateTime.Now}:[{threadId}][{logType.ToString().ToUpper()}]: {detail}");
            
            await writer.LogAsync(logType, message, detail);
            return null;
        }
    }
}