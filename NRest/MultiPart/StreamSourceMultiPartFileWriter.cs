using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRest.MultiPart
{
    internal class StreamSourceMultiPartFileWriter : IMultiPartFileWriter
    {
        private readonly Stream sourceStream;

        public StreamSourceMultiPartFileWriter(Stream stream)
        {
            this.sourceStream = stream;
        }

        public void Write(Stream stream)
        {
            sourceStream.CopyTo(stream);
        }

        public async Task WriteAsync(Stream stream)
        {
            await sourceStream.CopyToAsync(stream);
        }
    }
}
