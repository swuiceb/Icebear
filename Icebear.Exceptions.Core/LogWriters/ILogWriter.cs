using System;
using System.Threading.Tasks;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.LogWriters
{
    public interface ILogWriter
    {
        Task<ILogEntry> LogErrorAsync(Exception exception, params String[] tags);
        
        Task<ILogEntry> LogWarnAsync(Exception exception, params String[] tags);
        
        Task<ILogEntry> LogAsync<T>(LogType logType, string message, T detail, params String[] tags);
    }
}