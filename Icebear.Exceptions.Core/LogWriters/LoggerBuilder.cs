using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Icebear.Exceptions.Core.LogReaders;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.LogWriters.Providers;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.LogWriters
{
    public class LoggerBuilder
    {
        public class LoggerRepository
        {
            public ILogWriter Writer { get; internal set; }
            public ILogReader Reader { get; internal set; }
        }
        
        internal ILoggerRepository Repository { get; private set; }
        internal Func<Exception, ILogDescription> ExceptionTextProvider { get; private set; }
        internal Func<Exception, string> SourceProvider { get; private set; }
        internal Func<Exception, string> CodeTextProvider { get; private set; }

        private ILogWriter writer;
        private bool logConsole = true;
        private LogType consoleLevel = LogType.Info;
        // private string consoleFormat = "{--::TimeStamp::--}: [{--::Thread::--}][{--::Type::--}]: {--::Error::--}";
        private string consoleFormat = null;

        public LoggerBuilder WithRepository(ILoggerRepository repository)
        {
            this.Repository = repository;
            return this;
        }
        
        public LoggerBuilder WithExceptionTextProvider(Func<Exception, ILogDescription> exceptionTextProvider)
        {
            this.ExceptionTextProvider = exceptionTextProvider;
            return this;
        }

        public LoggerBuilder WithCodeTextProvider(
            Func<Exception, string> codeTextProvider)
        {
            this.CodeTextProvider = codeTextProvider;
            return this;
        }

        public LoggerBuilder WithSourceTextProvider(Func<Exception, string> sourceProvider)
        {
            this.SourceProvider = sourceProvider;
            return this;
        }

        public LoggerBuilder WithWriter(ILogWriter writer)
        {
            this.writer = writer;
            return this;
        }

        public LoggerBuilder WithConsoleLevel(LogType logLevel)
        {
            this.consoleLevel = logLevel;
            return this;
        }

        public LoggerBuilder WithConsoleFormat(string format)
        {
            consoleFormat = format;
            return this;
        }

        public LoggerBuilder DisableConsole()
        {
            this.logConsole = false;
            return this;
        }

        public ILogWriter BuildInMemory(int max)
        {
            return new InMemoryLogWriter(
                    ExceptionTextProvider,
                    SourceProvider,
                    CodeTextProvider)
                .Initialize(max);
        }

        public LoggerRepository Build(LogType logLevel)
        {
            if (logConsole)
            {
                var consoleWriter = new ConsoleLogWriter(
                    writer, 
                    consoleFormat,
                    ExceptionTextProvider, 
                    CodeTextProvider, 
                    SourceProvider);

                return
                    new LoggerRepository()
                    {
                        Writer = new LoggerWrapper(consoleLevel, consoleWriter),
                        Reader = writer as ILogReader
                    };
            }

            Debug.Assert(writer != null, nameof(writer) + " != null");
            return
                new LoggerRepository()
                {
                    Writer = new LoggerWrapper(logLevel, writer),
                    Reader = writer as ILogReader
                };
        }
    }
}