using System;
using System.Net.Mime;
using System.Text;
using Icebear.Exceptions.Core.Models;

namespace Icebear.Exceptions.Core.LogWriters.Providers
{
    public static class ExceptionTextProviders
    {
        public static IErrorDescription Default(Exception ex)
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

            return new ErrorDescription
            {
                Text = ex.Message,
                Description = sb.ToString()
            };
        }

        public static IErrorDescription Simple(Exception ex)
        {
            return new ErrorDescription
            {
                Text = ex.Message,
                Description = ex.StackTrace
            };
        }

        private record ErrorDescription : IErrorDescription
        {
            public String Text { get; init; }
            public String Description { get; init; }
        }
    }
}