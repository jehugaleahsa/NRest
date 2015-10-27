using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NRest
{
    public class RestClient
    {
        private readonly string baseUri;
        private readonly List<Action<IRequestConfiguration>> configurators;

        public RestClient()
        {
            this.configurators = new List<Action<IRequestConfiguration>>();
        }

        public RestClient(string baseUri)
        {
            this.baseUri = baseUri;
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
            return getConfiguration("GET", uriString, uriParameters);
        }

        public IRequestConfiguration Post(string uriString, object uriParameters = null)
        {
            return getConfiguration("POST", uriString, uriParameters);
        }

        public IRequestConfiguration Put(string uriString, object uriParameters = null)
        {
            return getConfiguration("PUT", uriString, uriParameters);
        }

        public IRequestConfiguration Delete(string uriString, object uriParameters = null)
        {
            return getConfiguration("DELETE", uriString, uriParameters);
        }

        public IRequestConfiguration Patch(string uriString, object uriParameters = null)
        {
            return getConfiguration("PATCH", uriString, uriParameters);
        }

        public IRequestConfiguration Head(string uriString, object uriParameters = null)
        {
            return getConfiguration("HEAD", uriString, uriParameters);
        }

        public IRequestConfiguration Options(string uriString, object uriParameters = null)
        {
            return getConfiguration("OPTIONS", uriString, uriParameters);
        }

        public IRequestConfiguration Trace(string uriString, object uriParameters = null)
        {
            return getConfiguration("TRACE", uriString, uriParameters);
        }

        public IRequestConfiguration Connect(string uriString, object uriParameters = null)
        {
            return getConfiguration("CONNECT", uriString, uriParameters);
        }

        private IRequestConfiguration getConfiguration(string method, string uriString, object uriParameters)
        {
            Uri uri = getUri(uriString, uriParameters);
            RequestConfiguration configuration = new RequestConfiguration(uri, method);
            foreach (var configurator in configurators)
            {
                configurator(configuration);
            }
            return configuration;
        }

        private Uri getUri(string uriPart, object uriParameters)
        {
            if (String.IsNullOrWhiteSpace(baseUri))
            {
                return applyParameters(uriPart, uriParameters);
            }
            if (String.IsNullOrWhiteSpace(uriPart))
            {
                return applyParameters(baseUri, uriParameters);
            }
            // This code will not work if someone passes in a scheme as base URL
            string uri = baseUri.TrimEnd('/') + "/" + uriPart.TrimStart('/');
            return applyParameters(uri, uriParameters);
        }

        private Uri applyParameters(string uri, object uriParameters)
        {
            if (uriParameters == null)
            {
                return new Uri(uri);
            }
            Type parameterType = uriParameters.GetType();
            var parameterLookup = parameterType.GetProperties().ToDictionary(p => p.Name, p => p.GetValue(uriParameters));
            foreach (string uriParameter in parameterLookup.Keys)
            {
                object value = parameterLookup[uriParameter];
                string encodedValue = value == null ? String.Empty : WebUtility.UrlEncode(value.ToString());
                uri = uri.Replace("{" + uriParameter + "}", encodedValue);
            }
            return new Uri(uri);
        }
    }
}
