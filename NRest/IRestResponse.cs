using System;
using System.Collections.Specialized;
using System.Net;

namespace NRest
{
    public interface IRestResponse
    {
        NameValueCollection Headers { get; }

        HttpStatusCode StatusCode { get; }

        bool IsSuccessStatusCode { get; }

        bool HasError { get; }

        string ReasonPhrase { get; }

        Version Version { get; }

        object Result { get; }

        TResult GetResult<TResult>();
    }
}
