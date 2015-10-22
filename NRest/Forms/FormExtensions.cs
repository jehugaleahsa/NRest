using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;

namespace NRest.Forms
{
    public static class FormExtensions
    {
        private const string contentType = "application/x-www-form-urlencoded";

        public static NameValueCollection FromForm(this HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string formData = reader.ReadToEnd();
                Uri fakeUri = new Uri("http://localhost/?" + formData);
                return fakeUri.ParseQueryString();
            }
        }

        public static IRequestConfiguration WithUrlEncodedBody(this IRequestConfiguration configuration, NameValueCollection collection)
        {
            return configuration.ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    string serialized = collection.ToQueryString();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                });
        }

        public static IRequestConfiguration WithUrlEncodedBody(this IRequestConfiguration configuration, Action<IUrlEncodedBodyBuilder> formBuilder)
        {
            return configuration.ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    UrlEncodedBodyBuilder builder = new UrlEncodedBodyBuilder();
                    formBuilder(builder);
                    NameValueCollection collection = builder.Collection;
                    string serialized = collection.ToQueryString();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                });
        }
    }
}
