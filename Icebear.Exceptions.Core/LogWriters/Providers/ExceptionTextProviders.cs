using System;
using System.Text;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.LogWriters.Providers
{
    public static class ExceptionTextProviders
    {
        public static ILogDescription Default(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Source);
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);

            if (!ex.Equals(ex.GetBaseException()))
            {
                sb.AppendLine("----Root cause---");

                var baseEx = ex.GetBaseException();
                sb.AppendLine(baseEx.Source);
                sb.AppendLine(baseEx.Message);
                sb.AppendLine(baseEx.StackTrace);
            }

            return new LogDescription
            {
                Text = ex.Message,
                Description = sb.ToString()
            };
        }

        public static ILogDescription Simple(Exception ex)
        {
            return new LogDescription
            {
                Text = ex.Message,
                Description = ex.StackTrace
            };
        }

        private class LogDescription : ILogDescription
        {
            public String Text { get; set; }
            public String Description { get; set; }
            
            public string UserContext { get; }
            public string SystemContext { get; }
        }
    }
}