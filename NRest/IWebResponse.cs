using System;
using System.Net;

namespace NRest
{
    public interface IWebResponse
    {
        HttpWebRequest Request { get; }

        HttpWebResponse Response { get; }
    }
}
