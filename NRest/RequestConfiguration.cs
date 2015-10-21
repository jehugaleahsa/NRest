using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NRest
{
    internal class RequestConfiguration : IRequestConfiguration
    {
        private readonly Uri uri;
        private readonly string method;
        private readonly Dictionary<int, Func<HttpWebResponse, object>> codeHandlers;
        private readonly NameValueCollection headers;
        private readonly NameValueCollection queryParameters;
        private ICredentials credentials;
        private Action<HttpWebRequest> configurator;
        private Func<HttpWebResponse, object> successHandler;
        private Func<HttpWebResponse, object> errorHandler;
        private Func<byte[]> bodyBuilder;

        public RequestConfiguration(Uri uri, string method)
        {
            this.uri = uri;
            this.method = method;
            this.codeHandlers = new Dictionary<int, Func<HttpWebResponse, object>>();
            this.headers = new NameValueCollection();
            this.queryParameters = new NameValueCollection();
        }

        public IRequestConfiguration WithCredentials(ICredentials credentials)
        {
            this.credentials = credentials;
            return this;
        }

        public IRequestConfiguration ConfigureRequest(Action<HttpWebRequest> configurator)
        {
            this.configurator = configurator;
            return this;
        }

        public IRequestConfiguration WithHeader(string name, string value)
        {
            this.headers.Add(name, value);
            return this;
        }

        public IRequestConfiguration WithQueryParameter(string name, string value)
        {
            this.queryParameters.Add(name, value);
            return this;
        }

        public IRequestConfiguration WithQueryParameter(string name, int? value)
        {
            this.queryParameters.Add(name, value.ToString());
            return this;
        }

        public IRequestConfiguration WithBody(Func<byte[]> body)
        {
            this.bodyBuilder = body;
            return this;
        }

        public IRequestConfiguration Success(Func<HttpWebResponse, object> handler)
        {
            this.successHandler = handler;
            return this;
        }

        public IRequestConfiguration Error(Func<HttpWebResponse, object> handler)
        {
            this.errorHandler = handler;
            return this;
        }

        public IRequestConfiguration When(int statusCode, Func<HttpWebResponse, object> handler)
        {
            this.codeHandlers[statusCode] = handler;
            return this;
        }

        public IRequestConfiguration When(HttpStatusCode statusCode, Func<HttpWebResponse, object> handler)
        {
            return When((int)statusCode, handler);
        }

        public IRestResponse Execute()
        {
            HttpWebRequest request = createRequest();
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return getSuccessResult(request, response);
                }
            }
            catch (WebException exception)
            {
                HttpWebResponse response = (HttpWebResponse)exception.Response;
                return getErrorResult(request, response);
            }
        }

        public async Task<IRestResponse> ExecuteAsync()
        {
            HttpWebRequest request = createRequest();
            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                return getSuccessResult(request, response);
            }
            catch (WebException exception)
            {
                HttpWebResponse response = (HttpWebResponse)exception.Response;
                return getErrorResult(request, response);
            }
        }

        private HttpWebRequest createRequest()
        {
            Uri fullUri = buildUri();
            HttpWebRequest request = HttpWebRequest.CreateHttp(fullUri);
            request.Method = method;
            request.Headers.Add(headers);
            if (credentials != null)
            {
                request.Credentials = credentials;
            }
            if (bodyBuilder != null)
            {
                buildbody(request);
            }
            if (configurator != null)
            {
                configurator(request);
            }
            return request;
        }

        private Uri buildUri()
        {
            UriBuilder builder = new UriBuilder(uri);
            if (queryParameters.Count > 0)
            {
                string queryString = getQueryString();
                builder.Query = queryString;
            }
            return builder.Uri;
        }

        private string getQueryString()
        {
            var pairs = from key in queryParameters.AllKeys
                        let encodedKey = WebUtility.UrlEncode(key)
                        from value in queryParameters.GetValues(key)
                        let encodedValue = WebUtility.UrlEncode(value)
                        select encodedKey + "=" + encodedValue;
            return String.Join("&", pairs);
        }

        private void buildbody(HttpWebRequest request)
        {
            byte[] rawBody = bodyBuilder();
            using (var stream = request.GetRequestStream())
            {
                stream.Write(rawBody, 0, rawBody.Length);
            }
        }

        private IRestResponse getSuccessResult(HttpWebRequest request, HttpWebResponse response)
        {
            RestResponse result = new RestResponse();
            result.StatusCode = response.StatusCode;
            result.HasError = false;
            if (codeHandlers.ContainsKey((int)response.StatusCode))
            {
                Func<HttpWebResponse, object> handler = codeHandlers[(int)response.StatusCode];
                result.Result = handler(response);
            }
            else if (successHandler != null)
            {
                result.Result = successHandler(response);
            }
            else
            {
                throw new RestException(request, "No success handler defined for the request.");
            }
            return result;
        }

        private IRestResponse getErrorResult(HttpWebRequest request, HttpWebResponse response)
        {
            RestResponse result = new RestResponse();
            result.StatusCode = response.StatusCode;
            result.HasError = true;
            if (codeHandlers.ContainsKey((int)response.StatusCode))
            {
                Func<HttpWebResponse, object> handler = codeHandlers[(int)response.StatusCode];
                result.Result = handler(response);
            }
            else if (errorHandler != null)
            {
                result.Result = errorHandler(response);
            }
            else
            {
                throw new RestException(request, "No error handler defined for the request.");
            }
            return result;
        }
    }
}
