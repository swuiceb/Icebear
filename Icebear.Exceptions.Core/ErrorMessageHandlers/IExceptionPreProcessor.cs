using System;

namespace Icebear.Exceptions.Core.ErrorMessageHandlers
{
    public interface IExceptionPreProcessor
    {
        Exception PreProcessException(Exception ex);
    }
}