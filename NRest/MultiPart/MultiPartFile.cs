using System;
using System.IO;

namespace NRest.MultiPart
{
    internal class MultiPartFile
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public Action<Stream> Writer { get; set; }

        public static Action<Stream> GetPathWriter(string path)
        {
            return stream =>
            {
                using (Stream sourceStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    sourceStream.CopyTo(stream);
                }
            };
        }

        public static Action<Stream> GetStreamWriter(Stream sourceStream)
        {
            return stream =>
            {
                sourceStream.CopyTo(stream);
            };
        }
    }
}
