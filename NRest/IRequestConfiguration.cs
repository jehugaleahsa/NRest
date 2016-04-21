using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace NRest
{
    public interface IRequestConfiguration
    {
        IRequestConfiguration WithCredentials(ICredentials credentials);

        IRequestConfiguration UsingDefaultCredentials(bool useDefault);

        IRequestConfiguration ConfigureRequest(Action<HttpWebRequest> configurator);

        IRequestConfiguration WithTimeout(int milliseconds);

        IRequestConfiguration WithTimeout(TimeSpan timeSpan);

        IRequestConfiguration WithHeader(string name, string value);

        IRequestConfiguration WithHeader(string name, int? value);

        IRequestConfiguration WithHeaders(NameValueCollection collection);

        IRequestConfiguration WithHeaders(object parameters);

        IRequestConfiguration WithQueryParameter(string name, string value);

        IRequestConfiguration WithQueryParameter(string name, int? value);

        IRequestConfiguration WithQueryParameters(NameValueCollection collection);

        IRequestConfiguration WithQueryParameters(object parameters);

        IRequestConfiguration WithBodyBuilder(IRequestBodyBuilder builder);

        IRequestConfiguration WhenSuccess(Func<IWebResponse, object> handler);

        IRequestConfiguration WhenError(Func<IWebResponse, object> handler);

        IRequestConfiguration When(int statusCode, Func<IWebResponse, object> handler);

        IRequestConfiguration When(HttpStatusCode statusCode, Func<IWebResponse, object> handler);

        IRequestConfiguration WhenUnhandled(Func<IWebResponse, object> handler);

        IRestResponse Execute();

        Task<IRestResponse> ExecuteAsync();
    }
}
