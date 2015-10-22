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
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream => 
                {
                    string serialized = JsonConvert.SerializeObject(body);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                });
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, Formatting formatting)
        {
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    string serialized = JsonConvert.SerializeObject(body, formatting);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                });
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, JsonSerializerSettings settings)
        {
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    string serialized = JsonConvert.SerializeObject(body, settings);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                });
        }

        public static IRequestConfiguration WithJsonBody(this IRequestConfiguration configuration, object body, Formatting formatting, JsonSerializerSettings settings)
        {
            return configuration
                .ConfigureRequest(r => r.ContentType = contentType)
                .WithBodyBuilder(stream =>
                {
                    string serialized = JsonConvert.SerializeObject(body, formatting, settings);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(serialized);
                });
        }

        public static TResult FromJson<TResult>(this HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string jsonData = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResult>(jsonData);
            }
        }

        public static TResult FromJson<TResult>(this HttpWebResponse response, JsonSerializerSettings settings)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string jsonData = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResult>(jsonData, settings);
            }
        }
    }
}
