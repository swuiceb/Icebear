using System;
using System.Diagnostics.CodeAnalysis;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Db.Ef.LogWriters.RollingDb;

namespace Icebear.Exceptions.Db.Ef.LogWriters
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
                    builder.CodeTextProvider);
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
                builder.CodeTextProvider);
        }
    }
}