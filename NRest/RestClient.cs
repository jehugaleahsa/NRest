using System;
using System.Collections.Generic;
using NRest.UriTemplates;

namespace NRest
{
    public class RestClient : IRestClient
    {
        private readonly string baseUri;
        private readonly List<Action<IRequestConfiguration>> configurators;

        public RestClient()
            : this(null)
        {
        }

        public RestClient(string baseUri)
        {
            this.baseUri = baseUri;
            this.configurators = new List<Action<IRequestConfiguration>>();
        }

        public void Configure(Action<IRequestConfiguration> configurator)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException("configurator");
            }
            this.configurators.Add(configurator);
        }

        public IRequestConfiguration Get(string uriString, object uriParameters = null)
        {
            return getConfiguration("GET", uriString, true, uriParameters);
        }

        public IRequestConfiguration Post(string uriString, object uriParameters = null)
        {
            return getConfiguration("POST", uriString, true, uriParameters);
        }

        public IRequestConfiguration Put(string uriString, object uriParameters = null)
        {
            return getConfiguration("PUT", uriString, true, uriParameters);
        }

        public IRequestConfiguration Delete(string uriString, object uriParameters = null)
        {
            return getConfiguration("DELETE", uriString, true, uriParameters);
        }

        public IRequestConfiguration Patch(string uriString, object uriParameters = null)
        {
            return getConfiguration("PATCH", uriString, true, uriParameters);
        }

        public IRequestConfiguration Head(string uriString, object uriParameters = null)
        {
            return getConfiguration("HEAD", uriString, true, uriParameters);
        }

        public IRequestConfiguration Options(string uriString, object uriParameters = null)
        {
            return getConfiguration("OPTIONS", uriString, true, uriParameters);
        }

        public IRequestConfiguration Trace(string uriString, object uriParameters = null)
        {
            return getConfiguration("TRACE", uriString, true, uriParameters);
        }

        public IRequestConfiguration Connect(string uriString, object uriParameters = null)
        {
            return getConfiguration("CONNECT", uriString, true, uriParameters);
        }

        public IRequestConfiguration CreateRequest(string method, string uriString, object uriParameters = null)
        {
            if (String.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("The method name cannot be null or blank.", "method");
            }
            return getConfiguration(method, uriString, true, uriParameters);
        }

        private IRequestConfiguration getConfiguration(string method, string uriString, bool caseSensitive, object uriParameters)
        {
            Uri uri = getUri(uriString, caseSensitive, uriParameters);
            RequestConfiguration configuration = new RequestConfiguration(uri, method);
            foreach (var configurator in configurators)
            {
                configurator(configuration);
            }
            return configuration;
        }

        private Uri getUri(string uriPart, bool caseSensitive, object uriParameters)
        {
            if (String.IsNullOrWhiteSpace(baseUri))
            {
                return applyParameters(uriPart, caseSensitive, uriParameters);
            }
            if (String.IsNullOrWhiteSpace(uriPart))
            {
                return applyParameters(baseUri, caseSensitive, uriParameters);
            }
            // This code will not work if someone passes in a scheme as base URL
            string uri = baseUri.TrimEnd('/') + "/" + uriPart.TrimStart('/');
            return applyParameters(uri, caseSensitive, uriParameters);
        }

        private Uri applyParameters(string uriTemplate, bool caseSensitive, object uriParameters)
        {
            UriTemplate template = new UriTemplate(uriTemplate, caseSensitive);
            string uri = template.Expand(uriParameters);
            return new Uri(uri);
        }
    }
}
