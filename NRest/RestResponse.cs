using System.Net;

namespace NRest
{
    internal class RestResponse : IRestResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool HasError { get; set; }

        public object Result { get; set; }

        public TResult GetResult<TResult>()
        {
            return (TResult)Result;
        }
    }
}
