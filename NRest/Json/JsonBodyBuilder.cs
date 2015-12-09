using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NRest.Json
{
    internal class JsonBodyBuilder : IRequestBodyBuilder
    {
        private readonly object body;
        private readonly Formatting formatting;
        private readonly JsonSerializerSettings settings;

        public JsonBodyBuilder(object body)
            : this(body, Formatting.None, new JsonSerializerSettings())
        {
        }

        public JsonBodyBuilder(object body, Formatting formatting)
            : this(body, formatting, new JsonSerializerSettings())
        {
        }

        public JsonBodyBuilder(object body, JsonSerializerSettings settings)
            : this(body, Formatting.None, settings)
        {
        }

        public JsonBodyBuilder(object body, Formatting formatting, JsonSerializerSettings settings)
        {
            this.body = body;
            this.formatting = formatting;
            this.settings = settings;
        }

        public void Build(Stream stream, Encoding encoding)
        {
            string serialized = JsonConvert.SerializeObject(body, formatting, settings);
            StreamWriter writer = new StreamWriter(stream, encoding);
            writer.Write(serialized);
        }

        public async Task BuildAsync(Stream stream, Encoding encoding)
        {
            string serialized = JsonConvert.SerializeObject(body, formatting, settings);
            StreamWriter writer = new StreamWriter(stream, encoding);
            await writer.WriteAsync(serialized);
        }
    }
}
