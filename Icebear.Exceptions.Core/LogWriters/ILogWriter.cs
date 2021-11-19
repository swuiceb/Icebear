using System;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.LogWriters
{
    public interface ILogWriter
    {
        Task<IError> LogErrorAsync(Exception exception);
        
        Task<IError> LogWarnAsync(Exception exception);
        
        Task<String> LogAsync<T>(LogType logType, string message, T detail);
    }
}