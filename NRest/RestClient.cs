using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace NRest
{
    public class RestClient
    {
        private readonly Uri baseUri;

        public RestClient()
        {
        }

        public RestClient(string baseUri)
        {
            this.baseUri = new Uri(baseUri);
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
            if (baseUri == null || String.IsNullOrEmpty(baseUri.AbsoluteUri))
            {
                return applyParameters(new Uri(uriPart), uriParameters);
            }
            if (String.IsNullOrWhiteSpace(uriPart))
            {
                return applyParameters(baseUri, uriParameters);
            }
            UriBuilder builder = new UriBuilder(baseUri);
            builder.Path = Regex.Replace(builder.Path + "/" + uriPart, @"/{2,}", @"/");
            return applyParameters(builder.Uri, uriParameters);
        }

        private Uri applyParameters(Uri uri, object uriParameters)
        {
            if (uriParameters == null)
            {
                return uri;
            }
            Type parameterType = uriParameters.GetType();
            var parameterLookup = parameterType.GetProperties().ToDictionary(p => p.Name, p => p.GetValue(uriParameters));
            UriBuilder builder = new UriBuilder(uri);
            string path = builder.Path;
            foreach (string uriParameter in parameterLookup.Keys)
            {
                object value = parameterLookup[uriParameter];
                string encodedValue = value == null ? String.Empty : WebUtility.UrlEncode(value.ToString());
                path = path.Replace("{" + uriParameter + "}", encodedValue);
            }
            builder.Path = path;
            return builder.Uri;
        }
    }
}
