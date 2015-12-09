using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NRest
{
    internal class StreamWriter
    {
        private readonly Stream stream;
        private readonly Encoding encoding;

        public StreamWriter(Stream stream, Encoding encoding)
        {
            this.stream = stream;
            this.encoding = encoding;
        }

        public Stream BaseStream
        {
            get { return stream; }
        }

        public void Write(string value)
        {
            if (value == null)
            {
                return;
            }
            byte[] data = encoding.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public async Task WriteAsync(string value)
        {
            if (value == null)
            {
                return;
            }
            byte[] data = encoding.GetBytes(value);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
