using System.Collections.Specialized;
using System.IO;

namespace NRest.MultiPart
{
    public class MultiPartSection
    {
        public NameValueCollection Headers { get; set; }

        public Stream Content { get; set; }
    }
}
