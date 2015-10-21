using System;
using System.Net;

namespace NRest
{
    public class RestException : Exception
    {
        private readonly WebRequest request;

        internal RestException(WebRequest request, string message)
            : base(message)
        {
            this.request = request;
        }
    }
}
