using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NRest
{
    public interface IRequestConfiguration
    {
        IRequestConfiguration WithCredentials(ICredentials credentials);

        IRequestConfiguration UsingDefaultCredentials(bool useDefault);

        IRequestConfiguration ConfigureRequest(Action<HttpWebRequest> configurator);

        IRequestConfiguration WithHeader(string name, string value);

        IRequestConfiguration WithQueryParameter(string name, string value);

        IRequestConfiguration WithQueryParameter(string name, int? value);

        IRequestConfiguration WithBodyBuilder(Action<Stream> body);

        IRequestConfiguration WhenSuccess(Func<HttpWebResponse, object> handler);

        IRequestConfiguration WhenError(Func<HttpWebResponse, object> handler);

        IRequestConfiguration When(int statusCode, Func<HttpWebResponse, object> handler);

        IRequestConfiguration When(HttpStatusCode statusCode, Func<HttpWebResponse, object> handler);

        IRequestConfiguration WhenUnhandled(Func<HttpWebResponse, object> handler);

        IRestResponse Execute();

        Task<IRestResponse> ExecuteAsync();
    }
}
