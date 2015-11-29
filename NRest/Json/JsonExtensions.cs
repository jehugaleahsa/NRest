using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace NRest.Json
{
    public static class JsonExtensions
    {
        public const string ContentType = "application/json";

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = ContentType)
                .WithBodyBuilder(new JsonBodyBuilder(body));
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, Formatting formatting)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = ContentType)
                .WithBodyBuilder(new JsonBodyBuilder(body, formatting));
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, JsonSerializerSettings settings)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = ContentType)
                .WithBodyBuilder(new JsonBodyBuilder(body, settings));
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, Formatting formatting, JsonSerializerSettings settings)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = ContentType)
                .WithBodyBuilder(new JsonBodyBuilder(body, formatting, settings));
        }

        public static TResult FromJson<TResult>(this IWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            using (var stream = response.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string jsonData = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResult>(jsonData);
            }
        }

        public static TResult FromJson<TResult>(this IWebResponse response, JsonSerializerSettings settings)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            using (var stream = response.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string jsonData = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResult>(jsonData, settings);
            }
        }
    }
}
