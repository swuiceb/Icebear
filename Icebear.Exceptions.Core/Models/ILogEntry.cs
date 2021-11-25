using System;

namespace Icebear.Exceptions.Core.Models
{
    public interface ILogEntry : ILogDescription
    {
        Guid Id { get; }
        String Code { get; }

        String Tags { get; }

        String Source { get; }
        
        DateTimeOffset OccurredDate { get; }
        
        LogType LogType { get; }
    }

    public interface ILogDescription
    {
        String Text { get; }
        
        String Description { get; }

        String UserContext { get; }
        
        String SystemContext { get; }
    }
}