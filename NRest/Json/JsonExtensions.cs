using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace NRest.Json
{
    public static class JsonExtensions
    {
        private const string contentType = "application/json";

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream => 
                {
                    string serialized = JsonConvert.SerializeObject(body);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                    writer.Flush();
                });
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, Formatting formatting)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    string serialized = JsonConvert.SerializeObject(body, formatting);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                    writer.Flush();
                });
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, JsonSerializerSettings settings)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    string serialized = JsonConvert.SerializeObject(body, settings);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                    writer.Flush();
                });
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, Formatting formatting, JsonSerializerSettings settings)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    string serialized = JsonConvert.SerializeObject(body, formatting, settings);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                    writer.Flush();
                });
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
