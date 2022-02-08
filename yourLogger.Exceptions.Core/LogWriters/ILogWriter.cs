using System;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Core.LogWriters
{
    public interface ILogWriter
    {
        ILogEntry LogError(Exception exception, params String[] tags);
        
        ILogEntry LogWarn(Exception exception, params String[] tags);
        
        ILogEntry Log<T>(LogType logType, string message, T detail, params String[] tags);
        
        Task<ILogEntry> LogErrorAsync(Exception exception, params String[] tags);
        
        Task<ILogEntry> LogWarnAsync(Exception exception, params String[] tags);
        
        Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail, params String[] tags);
    }
}