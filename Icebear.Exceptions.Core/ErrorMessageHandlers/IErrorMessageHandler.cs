using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Icebear.Exceptions.Core.ErrorMessageHandlers
{
    public interface IErrorMessageHandler
    {
        String GetFullExceptionText(Exception ex);
    }
}