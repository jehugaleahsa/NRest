using System.Collections.Specialized;

namespace NRest.MultiPart
{
    public class MultiPartResponse
    {
        public NameValueCollection FormData { get; internal set; }

        public MultiPartFileLookup Files { get; internal set; }
    }
}
