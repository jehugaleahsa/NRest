using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using NRest.ModelBinders;
using NRest.Reflection;

namespace NRest
{
    internal static class NameValueCollectionExtensionsInternal
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

    public static class NameValueCollectionExtensions
    {
        public static T Create<T>(this NameValueCollection collection)
            where T : new()
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            T instance = new T();
            NameValueCollectionModelBinder<T> binder = new NameValueCollectionModelBinder<T>(collection);
            binder.Bind(instance);
            return instance;
        }

        public static void Update<T>(this NameValueCollection collection, T instance)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            NameValueCollectionModelBinder<T> binder = new NameValueCollectionModelBinder<T>(collection);
            binder.Bind(instance);
        }

        internal static NameValueCollection CreateNameValueCollection(object instance)
        {
            NameValueCollection collection = new NameValueCollection();
            IPropertyLookup lookup = PropertyLookup.CreatePropertyLookup(instance);
            foreach (IProperty property in lookup.GetProperties())
            {
                var values = PropertyLookup.GetValues(property, false)
                    .Where(v => !(v is Tuple<string, object>));
                foreach (object value in values)
                {
                    collection.Add(property.Name, escape(value));
                }
            }
            return collection;
        }

        private static string escape(object value)
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }
    }
}
