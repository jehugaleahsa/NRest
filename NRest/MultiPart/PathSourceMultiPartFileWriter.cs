using System.IO;
using System.Threading.Tasks;

namespace NRest.MultiPart
{
    internal class PathSourceMultiPartFileWriter : IMultiPartFileWriter
    {
        private readonly string path;

        public PathSourceMultiPartFileWriter(string path)
        {
            this.path = path;
        }

        public void Write(Stream stream)
        {
            using (FileStream sourceStream = File.OpenRead(path))
            {
                sourceStream.CopyTo(stream);
            }
        }

        public async Task WriteAsync(Stream stream)
        {
            using (Stream sourceStream = File.OpenRead(path))
            {
                await sourceStream.CopyToAsync(stream);
            }
        }
    }
}
