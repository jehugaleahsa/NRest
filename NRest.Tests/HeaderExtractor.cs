using System.Collections.Specialized;
using System.Net;
using FakeServers.Extractors;

namespace NRest.Tests
{
    public class HeaderExtractor : IRequestBodyExtractor
    {
        public NameValueCollection Headers { get; private set; }

        public void Extract(HttpListenerRequest request)
        {
            Headers = new NameValueCollection(request.Headers);
        }
    }
}
