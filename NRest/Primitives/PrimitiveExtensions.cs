using System;
using System.IO;
using System.Net;
using System.Text;

namespace NRest.Primitives
{
    public static class PrimitiveExtensions
    {
        public static T FromString<T>(this IWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            string asString = getStringResponse(response.Response);
            if (String.IsNullOrWhiteSpace(asString))
            {
                return default(T);
            }
            Type type = typeof(T);
            Type nullableType = Nullable.GetUnderlyingType(type);
            Type castType = nullableType ?? type;
            return (T)Convert.ChangeType(asString, castType);
        }

        private static string getStringResponse(HttpWebResponse response)
        {
            Encoding encoding = response.CharacterSet == null ? Encoding.UTF8 : Encoding.GetEncoding(response.CharacterSet);
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, encoding))
            {
                string value = reader.ReadToEnd();
                return value;
            }
        }
    }
}
