using System;
using System.Net;
using System.Threading.Tasks;

namespace NRest
{
    public interface IRequestConfiguration
    {
        IRequestConfiguration WithCredentials(ICredentials credentials);

        IRequestConfiguration ConfigureRequest(Action<HttpWebRequest> configurator);

        IRequestConfiguration WithHeader(string name, string value);

        IRequestConfiguration WithQueryParameter(string name, string value);

        IRequestConfiguration WithQueryParameter(string name, int? value);

        IRequestConfiguration WithBody(Func<byte[]> body);

        IRequestConfiguration Success(Func<HttpWebResponse, object> handler);

        IRequestConfiguration Error(Func<HttpWebResponse, object> handler);

        IRequestConfiguration When(int statusCode, Func<HttpWebResponse, object> handler);

        IRequestConfiguration When(HttpStatusCode statusCode, Func<HttpWebResponse, object> handler);

        IRequestConfiguration Else(Func<HttpWebResponse, object> handler);

        IRestResponse Execute();

        Task<IRestResponse> ExecuteAsync();
    }
}
