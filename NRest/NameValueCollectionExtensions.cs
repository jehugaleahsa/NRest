using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace NRest
{
    internal static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection collection)
        {
            var pairs = from key in collection.AllKeys
                        let encodedKey = WebUtility.UrlEncode(key)
                        from value in collection.GetValues(key)
                        let encodedValue = WebUtility.UrlEncode(value)
                        select encodedKey + "=" + encodedValue;
            return String.Join("&", pairs);
        }
    }
}
