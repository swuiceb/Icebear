using System;

namespace yourLogs.Exceptions.Core.ErrorMessageHandlers
{
    public interface IExceptionPreProcessor
    {
        Exception PreProcessException(Exception ex);
    }
}