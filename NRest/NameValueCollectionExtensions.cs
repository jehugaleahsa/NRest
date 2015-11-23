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

        public static NameValueCollection ParseQueryString(this Uri uri)
        {
            string queryString = uri.Query;
            if (String.IsNullOrWhiteSpace(queryString))
            {
                return new NameValueCollection();
            }
            char[] parameterSeparators = new char[] { '&' };
            char[] pairSeparators = new char[] { '=' };
            string[] parameters = queryString.Split(parameterSeparators, StringSplitOptions.RemoveEmptyEntries);
            var keyValuePairs = from parameter in parameters
                                let parts = parameter.Split(pairSeparators, 2)
                                where parts.Length == 2
                                select new
                                {
                                    Key = String.IsNullOrWhiteSpace(parts[0]) ? null : WebUtility.UrlDecode(parts[0]),
                                    Value = String.IsNullOrWhiteSpace(parts[0]) ? null : WebUtility.UrlDecode(parts[1])
                                };
            NameValueCollection collection = new NameValueCollection();
            foreach (var pair in keyValuePairs)
            {
                collection.Add(pair.Key, pair.Value);
            }
            return collection;
        }
    }
}
