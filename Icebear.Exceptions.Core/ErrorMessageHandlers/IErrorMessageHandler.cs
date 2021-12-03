using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace yourLogs.Exceptions.Core.ErrorMessageHandlers
{
    public interface IErrorMessageHandler
    {
        String GetFullExceptionText(Exception ex);
    }
}