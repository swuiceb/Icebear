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
            [NotNull] Func<ErrorDbContext> contextProvider)
        {
                return DbLogWriter.Create(
                    contextProvider,
                    builder.ExceptionTextProvider,
                    builder.SourceProvider,
                    builder.CodeTextProvider);
        }

        public static ILogWriter BuildRollingDb(
            [NotNull]this LoggerBuilder builder,
            [NotNull]Func<ErrorDbContext> contextProvider,
            int batchSize
            )
        {
            return new RollingDbLogWriter(
                batchSize,
                contextProvider,
                builder.ExceptionTextProvider,
                builder.SourceProvider,
                builder.CodeTextProvider);
        }
    }
}