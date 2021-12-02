using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Core.LogWriters
{
    internal class ConsoleLogWriter : LogWriterBase, ILogWriter
    {
        private readonly ILogWriter writer;
        private readonly string format;

        internal ConsoleLogWriter(ILogWriter writer,
            string format,
            Func<Exception, ILogDescription> exceptionTextProvider,
            Func<Exception, string> codeTextProvider,
            Func<Exception, string> sourceProvider) 
            : base(exceptionTextProvider, sourceProvider: codeTextProvider, codeProvider: sourceProvider)
        {
            this.writer = writer;
            this.format = format;
        }

        public async Task<ILogEntry> LogErrorAsync(Exception exception, params String[] tags)
        {
            var threadId = 0; // Thread.CurrentThread.ManagedThreadId;
            // TODO: if a format is given, respect the format
            Console.WriteLine($"{DateTime.Now}:[{threadId}][ERROR]: {exception}");
            return await writer.LogErrorAsync(exception, tags);
        }

        public async Task<ILogEntry> LogWarnAsync(Exception exception,params String[] tags)
        {
            var threadId = 0; //Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{DateTime.Now}:[{threadId}][WARN]: {exception}");
            return await writer.LogWarnAsync(exception, tags);
        }

        public async Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail,params String[] tags)
        {
            var threadId = 0;//Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{DateTime.Now}:[{threadId}][{logType.ToString().ToUpper()}]: {message}");
            Console.WriteLine($"{DateTime.Now}:[{threadId}][{logType.ToString().ToUpper()}]: {detail}");
            
            return await writer.LogAsync(logType, message, detail, tags);
        }
    }
}