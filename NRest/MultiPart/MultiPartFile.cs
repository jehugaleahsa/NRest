using System;
using System.IO;

namespace NRest.MultiPart
{
    public class MultiPartFile
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Contents { get; set; }

        internal Action<Stream> Writer { get; set; }

        internal static Action<Stream> GetPathWriter(string path)
        {
            return stream =>
            {
                using (Stream sourceStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    sourceStream.CopyTo(stream);
                }
            };
        }

        internal static Action<Stream> GetStreamWriter(Stream sourceStream)
        {
            return stream =>
            {
                sourceStream.CopyTo(stream);
            };
        }
    }
}
