using System;
using System.Net;

namespace NRest
{
    internal class WebResponse : IWebResponse
    {
        public HttpWebRequest Request { get; set; }

        public HttpWebResponse Response { get; set; }
    }
}
