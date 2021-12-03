using System;

namespace yourLogs.Exceptions.Core.Tests.ExceptionProcessors
{
    public static class ExceptionUtilities
    {
        public static Exception GetNestedException(String inner, String outer)
        {
            try
            {
                ThrowNestedException(inner, outer);
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }
        
        public static void ThrowNestedException(String inner, String outer)
        {
            try
            {
                InnerException();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(outer, ex);
            }


            void InnerException()
            {
                throw new Exception(inner);
            }
        }
    }
}