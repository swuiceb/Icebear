using System;

namespace Icebear.Exceptions.Core.Models
{
    public interface IError : IErrorDescription
    {
        String Id { get; }
        String Code { get; }
        
        String Source { get; }
        
        DateTimeOffset OccurredDate { get; }
        
        LogType LogType { get; }
    }

    public interface IErrorDescription
    {
        String Text { get; }
        
        String Description { get; }
        
    }
}