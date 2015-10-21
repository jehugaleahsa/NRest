using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace NRest
{
    public class RestClient
    {
        private readonly string baseUri;

        public RestClient()
        {
        }

        public RestClient(string baseUri)
        {
            this.baseUri = baseUri;
        }

        public IRequestConfiguration Get(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "GET");
        }

        public IRequestConfiguration Post(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "POST");
        }

        public IRequestConfiguration Put(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "PUT");
        }

        public IRequestConfiguration Delete(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "DELETE");
        }

        public IRequestConfiguration Patch(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "PATCH");
        }

        public IRequestConfiguration Head(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "HEAD");
        }

        public IRequestConfiguration Options(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "OPTIONS");
        }

        public IRequestConfiguration Trace(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "TRACE");
        }

        public IRequestConfiguration Connect(string uriString, object uriParameters = null)
        {
            Uri uri = getUri(uriString, uriParameters);
            return new RequestConfiguration(uri, "CONNECT");
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
            string uri = baseUri;
            if (!uri.EndsWith("/") && !uri.StartsWith(uriPart))
            {
                uri += "/";
            }
            uri += uriPart;
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
