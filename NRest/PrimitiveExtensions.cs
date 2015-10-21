using System;
using System.IO;
using System.Net;

namespace NRest.Primitives
{
    public static class PrimitiveExtensions
    {
        public static T FromString<T>(this HttpWebResponse response)
        {
            string asString = getStringResponse(response);
            return (T)Convert.ChangeType(asString, typeof(T));
        }

        private static string getStringResponse(HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string value = reader.ReadToEnd();
                return value;
            }
        }
    }
}
