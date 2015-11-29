using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace NRest.ModelBinders
{
    public class NameValueCollectionValueProvider : IValueProvider
    {
        private static readonly HashSet<Type> types = new HashSet<Type>() 
        { 
            typeof(String),
            typeof(Byte),
            typeof(Byte?),
            typeof(Int16),
            typeof(Int16?),
            typeof(Int32),
            typeof(Int32?),
            typeof(Int64),
            typeof(Int64?),
            typeof(Single),
            typeof(Single?),
            typeof(Double),
            typeof(Double?),
            typeof(Decimal),
            typeof(Decimal?),
            typeof(DateTime), 
            typeof(DateTime?) 
        };

        private readonly NameValueCollection collection;
        private readonly HashSet<string> keys;

        public NameValueCollectionValueProvider(NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.collection = collection;
            this.keys = new HashSet<string>(collection.AllKeys, StringComparer.CurrentCultureIgnoreCase);
        }

        public static bool IsSupportedType(Type type)
        {
            if (types.Contains(type))
            {
                return true;
            }
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public ValueProviderResult GetValue(Type type, string key)
        {
            return GetValue(type, key, null);
        }

        public ValueProviderResult GetValue(Type type, string key, CultureInfo cultureInfo)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!keys.Contains(key))
            {
                return new ValueProviderResult() { IsSuccess = false };
            }
            if (type != typeof(String) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                return setCollection(type, key, cultureInfo);
            }
            else
            {
                return setPrimitive(type, key, cultureInfo);
            }
        }

        private ValueProviderResult setCollection(Type type, string key, CultureInfo cultureInfo)
        {
            CollectionDetails details = getCollection(type);
            if (details == null)
            {
                return new ValueProviderResult() { IsSuccess = false };
            }
            dynamic results = details.Collection;
            foreach (string rawValue in collection.GetValues(key))
            {
                dynamic value;
                if (tryGetPrimitive(details.ArgumentType, rawValue, cultureInfo, out value))
                {
                    results.Add(value);
                }
            }
            if (details.IsArray)
            {
                results = results.ToArray();
            }
            return new ValueProviderResult() { IsSuccess = true, RawValue = collection.Get(key), Value = results }; 
        }

        private static CollectionDetails getCollection(Type type)
        {
            if (type.IsInterface)
            {
                if (!type.IsGenericType)
                {
                    return new CollectionDetails() { Collection = new List<string>(), ArgumentType = typeof(string) };
                }
                Type argumentType = null;
                argumentType = getGenericArgument(type, typeof(ISet<>));
                if (argumentType != null)
                {
                    dynamic results = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(argumentType));
                    return new CollectionDetails() { Collection = results, ArgumentType = argumentType };
                }
                argumentType = getGenericArgument(type,
                        typeof(IEnumerable<>),
                        typeof(ICollection<>),
                        typeof(IList<>));
                if (argumentType != null)
                {
                    dynamic results = Activator.CreateInstance(typeof(List<>).MakeGenericType(argumentType));
                    return new CollectionDetails() { Collection = results, ArgumentType = argumentType };
                }
                return null;
            }
            else if (!type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
            {
                Type argumentType = getGenericArgument(type, typeof(ICollection<>));
                if (argumentType == null)
                {
                    return null;
                }
                dynamic results = Activator.CreateInstance(type);
                return new CollectionDetails() { Collection = results, ArgumentType = argumentType };
            }
            else if (type.IsArray)
            {
                Type argumentType = getGenericArgument(type, typeof(ICollection<>));
                if (argumentType == null)
                {
                    return null;
                }
                dynamic results = Activator.CreateInstance(typeof(List<>).MakeGenericType(argumentType));
                return new CollectionDetails() { Collection = results, ArgumentType = argumentType, IsArray = true };
            }
            return null;
        }

        private static Type getGenericArgument(Type collectionType, params Type[] baseTypes)
        {
            HashSet<Type> interfaces = new HashSet<Type>();
            if (collectionType.IsInterface)
            {
                interfaces.Add(collectionType);
            }
            interfaces.UnionWith(collectionType.GetInterfaces());
            var argumentTypes = from type in interfaces
                                where type.IsGenericType
                                let arguments = type.GetGenericArguments()
                                where arguments.Length == 1
                                let argument = arguments.Single()
                                where IsSupportedType(argument)
                                let typedCollections = from baseType in baseTypes
                                                       let typedCollection = baseType.MakeGenericType(argument)
                                                       where typedCollection.IsAssignableFrom(type)
                                                       select typedCollection
                                where !baseTypes.Any() || typedCollections.Any()
                                select argument;
            return argumentTypes.FirstOrDefault();
        }

        private ValueProviderResult setPrimitive(Type type, string key, CultureInfo cultureInfo)
        {
            string rawValue = collection[key];
            object value;
            if (tryGetPrimitive(type, rawValue, cultureInfo, out value))
            {
                return new ValueProviderResult() { IsSuccess = true, RawValue = rawValue, Value = value };
            }
            else
            {
                return new ValueProviderResult() { IsSuccess = false };
            }
        }

        private static bool tryGetPrimitive(Type primitiveType, string rawValue, CultureInfo cultureInfo, out object value)
        {
            if (primitiveType == typeof(String))
            {
                // If we are dealing with a String, just return what's in the collection.
                value = rawValue;
                return true;
            }
            if (String.IsNullOrWhiteSpace(rawValue))
            {
                // Otherwise, just treat blank strings as null.
                rawValue = null;
            }
            Type underlyingType = Nullable.GetUnderlyingType(primitiveType);
            if (rawValue == null)
            {
                // Null is only a valid value if we are dealing with a nullable type.
                value = null;
                return underlyingType != null;
            }
            else
            {
                Type actualType = underlyingType ?? primitiveType;
                return tryConvert(actualType, rawValue, cultureInfo, out value);
            }
        }

        private static bool tryConvert(Type type, string rawValue, CultureInfo cultureInfo, out object convertedValue)
        {
            try
            {
                convertedValue = Convert.ChangeType(rawValue, type, cultureInfo ?? CultureInfo.CurrentCulture);
                return true;
            }
            catch
            {
                convertedValue = null;
                return false;
            }
        }

        private class CollectionDetails
        {
            public object Collection { get; set; }

            public Type ArgumentType { get; set; }

            public bool IsArray { get; set; }
        }
    }
}
