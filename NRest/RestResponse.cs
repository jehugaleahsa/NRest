using System;
using System.Collections.Specialized;
using System.Net;

namespace NRest
{
    internal class RestResponse : IRestResponse
    {
        public NameValueCollection Headers { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode { get; set; }

        public bool HasError { get; set; }

        public string ReasonPhrase { get; set; }

        public Version Version { get; set; }

        public object Result { get; set; }

        public TResult GetResult<TResult>()
        {
            return (TResult)Result;
        }
    }
}
