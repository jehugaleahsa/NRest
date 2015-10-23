using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
        private readonly List<Action<HttpWebRequest>> configurators;
        private bool? useDefaultCredentials;
        private ICredentials credentials;
        private Func<HttpWebResponse, object> successHandler;
        private Func<HttpWebResponse, object> errorHandler;
        private Func<HttpWebResponse, object> unhandledHandler;
        private Action<Stream> bodyBuilder;

        public RequestConfiguration(Uri uri, string method)
        {
            this.uri = uri;
            this.method = method;
            this.codeHandlers = new Dictionary<int, Func<HttpWebResponse, object>>();
            this.headers = new NameValueCollection();
            this.queryParameters = new NameValueCollection();
            this.configurators = new List<Action<HttpWebRequest>>();
        }

        public IRequestConfiguration WithCredentials(ICredentials credentials)
        {
            this.credentials = credentials;
            return this;
        }

        public IRequestConfiguration UsingDefaultCredentials(bool useDefault)
        {
            this.useDefaultCredentials = useDefault;
            return this;
        }

        public IRequestConfiguration ConfigureRequest(Action<HttpWebRequest> configurator)
        {
            this.configurators.Add(configurator);
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

        public IRequestConfiguration WithBodyBuilder(Action<Stream> body)
        {
            this.bodyBuilder = body;
            return this;
        }

        public IRequestConfiguration WhenSuccess(Func<HttpWebResponse, object> handler)
        {
            this.successHandler = handler;
            return this;
        }

        public IRequestConfiguration WhenError(Func<HttpWebResponse, object> handler)
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

        public IRequestConfiguration WhenUnhandled(Func<HttpWebResponse, object> handler)
        {
            this.unhandledHandler = handler;
            return this;
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
                return getErrorResult(request, exception);
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
                return getErrorResult(request, exception);
            }
        }

        private HttpWebRequest createRequest()
        {
            Uri fullUri = buildUri();
            HttpWebRequest request = HttpWebRequest.CreateHttp(fullUri);
            request.Method = method;
            setHeaders(request);
            if (useDefaultCredentials != null)
            {
                request.UseDefaultCredentials = useDefaultCredentials.Value;
            }
            if (credentials != null)
            {
                request.Credentials = credentials;
            }
            foreach (var configurator in configurators)
            {
                configurator(request);
            }
            if (bodyBuilder != null)
            {
                buildbody(request);
            }
            return request;
        }

        private Uri buildUri()
        {
            UriBuilder builder = new UriBuilder(uri);
            if (queryParameters.Count > 0)
            {
                builder.Query = queryParameters.ToQueryString();
            }
            return builder.Uri;
        }

        private void setHeaders(HttpWebRequest request)
        {
            Dictionary<string, Action<string>> specialHeaders = new Dictionary<string, Action<string>>
            {
                { "Accept", x => request.Accept = x },
                { "Content-Type", x => request.ContentType = x },
                { "Connection", x => request.Connection = x },
                { "Content-Length", x => 
                    {
                        long length;
                        if (Int64.TryParse(x, out length))
                        {
                            request.ContentLength = length;
                        }
                    }
                },
                { "Expect", x => request.Expect = x },
                { "Host", x => request.Host = x },
                { "Referer", x => request.Referer = x },
                { "Transfer-Encoding", x => request.TransferEncoding = x },
                { "User-Agent", x => request.UserAgent = x },
                { "If-Modified-Since", x => 
                    {
                        DateTime date;
                        if (DateTime.TryParse(x, out date))
                        {
                            request.IfModifiedSince = date;
                        }
                    }
                },
                { "Date", x =>
                    {
                        DateTime date;
                        if (DateTime.TryParse(x, out date))
                        {
                            request.Date = date;
                        }
                    }
                }
            };
            NameValueCollection headersCopy = new NameValueCollection(headers);
            foreach (string header in specialHeaders.Keys)
            {
                string value = headersCopy.Get(header);
                if (value != null)
                {                    
                    specialHeaders[header](value);
                    headersCopy.Remove(header);
                }
            }
            request.Headers.Add(headersCopy);
        }

        private void buildbody(HttpWebRequest request)
        {
            using (var stream = request.GetRequestStream())
            {
                bodyBuilder(stream);
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
            else if (unhandledHandler != null)
            {
                result.Result = unhandledHandler(response);
            }
            return result;
        }

        private IRestResponse getErrorResult(HttpWebRequest request, WebException exception)
        {
            RestResponse result = new RestResponse();
            HttpWebResponse response = (HttpWebResponse)exception.Response;
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
            else if (unhandledHandler != null)
            {
                result.Result = unhandledHandler(response);
            }
            return result;
        }
    }
}
