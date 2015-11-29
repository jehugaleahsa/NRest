using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NRest
{
    internal class RequestConfiguration : IRequestConfiguration
    {
        private readonly Uri uri;
        private readonly string method;
        private readonly Dictionary<int, Func<IWebResponse, object>> codeHandlers;
        private readonly NameValueCollection headers;
        private readonly NameValueCollection queryParameters;
        private readonly List<Action<HttpWebRequest>> configurators;
        private bool? useDefaultCredentials;
        private ICredentials credentials;
        private Func<IWebResponse, object> successHandler;
        private Func<IWebResponse, object> errorHandler;
        private Func<IWebResponse, object> unhandledHandler;
        private IRequestBodyBuilder bodyBuilder;

        public RequestConfiguration(Uri uri, string method)
        {
            this.uri = uri;
            this.method = method;
            this.codeHandlers = new Dictionary<int, Func<IWebResponse, object>>();
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
            if (configurator == null)
            {
                throw new ArgumentNullException("configurator");
            }
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

        public IRequestConfiguration WithQueryParameters(NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.queryParameters.Add(collection);
            return this;
        }

        public IRequestConfiguration WithBodyBuilder(IRequestBodyBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            this.bodyBuilder = builder;
            return this;
        }

        public IRequestConfiguration WhenSuccess(Func<IWebResponse, object> handler)
        {
            this.successHandler = handler;
            return this;
        }

        public IRequestConfiguration WhenError(Func<IWebResponse, object> handler)
        {
            this.errorHandler = handler;
            return this;
        }

        public IRequestConfiguration When(int statusCode, Func<IWebResponse, object> handler)
        {
            if (handler == null)
            {
                this.codeHandlers.Remove(statusCode);
            }
            else
            {
                this.codeHandlers[statusCode] = handler;
            }
            return this;
        }

        public IRequestConfiguration When(HttpStatusCode statusCode, Func<IWebResponse, object> handler)
        {
            return When((int)statusCode, handler);
        }

        public IRequestConfiguration WhenUnhandled(Func<IWebResponse, object> handler)
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

        private HttpWebRequest createRequest()
        {
            HttpWebRequest request = createEmptyRequest();
            if (bodyBuilder != null)
            {
                buildbody(request);
            }
            return request;
        }

        private void buildbody(HttpWebRequest request)
        {
            using (var stream = request.GetRequestStream())
            {
                bodyBuilder.Build(stream);
            }
        }

        public async Task<IRestResponse> ExecuteAsync()
        {
            HttpWebRequest request = await createRequestAsync();
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

        private async Task<HttpWebRequest> createRequestAsync()
        {
            HttpWebRequest request = createEmptyRequest();
            if (bodyBuilder != null)
            {
                await buildbodyAsync(request);
            }
            return request;
        }

        private async Task buildbodyAsync(HttpWebRequest request)
        {
            using (var stream = await request.GetRequestStreamAsync())
            {
                await bodyBuilder.BuildAsync(stream);
            }
        }

        private HttpWebRequest createEmptyRequest()
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

        private IRestResponse getSuccessResult(HttpWebRequest request, HttpWebResponse response)
        {
            RestResponse result = new RestResponse();
            result.StatusCode = response.StatusCode;
            result.HasError = false;
            WebResponse webResponse = new WebResponse()
            {
                Request = request,
                Response = response,
                Exception = null
            };
            if (codeHandlers.ContainsKey((int)response.StatusCode))
            {
                Func<IWebResponse, object> handler = codeHandlers[(int)response.StatusCode];
                result.Result = handler(webResponse);
            }
            else if (successHandler != null)
            {
                result.Result = successHandler(webResponse);
            }
            else if (unhandledHandler != null)
            {
                result.Result = unhandledHandler(webResponse);
            }
            return result;
        }

        private IRestResponse getErrorResult(HttpWebRequest request, WebException exception)
        {
            RestResponse result = new RestResponse();
            HttpWebResponse response = (HttpWebResponse)exception.Response;
            if (response == null)
            {
                throw new RestException(request, "An error occurred while processing the request.", exception);
            }
            result.StatusCode = response.StatusCode;
            result.HasError = true;
            WebResponse webResponse = new WebResponse()
            {
                Request = request,
                Response = response,
                Exception = exception
            };
            if (codeHandlers.ContainsKey((int)response.StatusCode))
            {
                Func<IWebResponse, object> handler = codeHandlers[(int)response.StatusCode];
                result.Result = handler(webResponse);
            }
            else if (errorHandler != null)
            {
                result.Result = errorHandler(webResponse);
            }
            else if (unhandledHandler != null)
            {
                result.Result = unhandledHandler(webResponse);
            }
            return result;
        }
    }
}
