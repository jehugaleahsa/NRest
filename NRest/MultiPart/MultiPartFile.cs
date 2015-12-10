using System.Collections.Specialized;
using System.IO;

namespace NRest.MultiPart
{
    public class MultiPartFile
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Contents { get; set; }

        internal IMultiPartFileWriter Writer { get; set; }

        internal NameValueCollection Headers { get; set; }

        internal static IMultiPartFileWriter GetPathWriter(string path)
        {
            return new PathSourceMultiPartFileWriter(path);
        }

        internal static IMultiPartFileWriter GetStreamWriter(Stream sourceStream)
        {
            return new StreamSourceMultiPartFileWriter(sourceStream);
        }
    }
}
