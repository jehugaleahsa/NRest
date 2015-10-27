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

        IRequestConfiguration WhenSuccess(Func<IWebResponse, object> handler);

        IRequestConfiguration WhenError(Func<IWebResponse, object> handler);

        IRequestConfiguration When(int statusCode, Func<IWebResponse, object> handler);

        IRequestConfiguration When(HttpStatusCode statusCode, Func<IWebResponse, object> handler);

        IRequestConfiguration WhenUnhandled(Func<IWebResponse, object> handler);

        IRestResponse Execute();

        Task<IRestResponse> ExecuteAsync();
    }
}
