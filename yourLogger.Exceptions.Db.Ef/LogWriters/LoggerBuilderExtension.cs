using System;
using System.Diagnostics.CodeAnalysis;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Core.Repository;
using yourLogs.Exceptions.Db.Ef.LogWriters.RollingDb;

namespace yourLogs.Exceptions.Db.Ef.LogWriters
{
    public static class LoggerBuilderExtension
    {
        public static ILogWriter BuildInDb(
            [NotNull]this LoggerBuilder builder,
            [NotNull]ILoggerRepository repository)
        {
                return DbLogWriter.Create(
                    repository,
                    builder.ExceptionTextProvider,
                    builder.SourceProvider,
                    builder.CodeTextProvider,
                    builder.SystemContextProvider);
        }
        
        public static ILogWriter BuildRollingDb(
            [NotNull]this LoggerBuilder builder,
            [NotNull]ILoggerRepository repository,
            int batchSize
            )
        {
            return new RollingDbLogWriter(
                batchSize,
                repository,
                builder.ExceptionTextProvider,
                builder.SourceProvider,
                builder.CodeTextProvider,
                builder.SystemContextProvider);
        }
    }
}