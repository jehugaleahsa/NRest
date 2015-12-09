using System.Net;

namespace NRest
{
    public interface IRestResponse
    {
        HttpStatusCode StatusCode { get; }

        bool HasError { get; }

        object Result { get; }

        TResult GetResult<TResult>();
    }
}
