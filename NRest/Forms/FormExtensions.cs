using System;
using System.Collections.Specialized;
using System.IO;

namespace NRest.Forms
{
    public static class FormExtensions
    {
        public const string ContentType = "application/x-www-form-urlencoded";

        public static NameValueCollection FromForm(this IWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            using (var stream = response.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string formData = reader.ReadToEnd();
                Uri fakeUri = new Uri("http://localhost/?" + formData);
                return fakeUri.ParseQueryString();
            }
        }

        public static IRequestConfiguration WithUrlEncodedBody(this IRequestConfiguration configuration, NameValueCollection collection)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = ContentType)
                .WithBodyBuilder(new UrlEncodedBodyBuilder(collection));
        }

        public static IRequestConfiguration WithUrlEncodedBody(this IRequestConfiguration configuration, Action<IUrlEncodedBodyBuilder> formBuilder)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (formBuilder == null)
            {
                throw new ArgumentNullException("formBuilder");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = ContentType)
                .WithBodyBuilder(new UrlEncodedBodyBuilder(formBuilder));
        }
    }
}
