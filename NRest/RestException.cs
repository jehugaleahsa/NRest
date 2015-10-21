using System;
using System.Net;

namespace NRest
{
    public class RestException : Exception
    {
        internal RestException(WebRequest request, string message)
            : base(message)
        {
            WebRequest = request;
        }

        internal RestException(WebRequest request, string message, Exception innerException)
            : base(message, innerException)
        {
            WebRequest = request;
        }

        public WebRequest WebRequest { get; private set; }
    }
}
