using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace NRest.UriTemplates
{
    internal class PlaceholderGenerator : ISectionGenerator
    {
        public PlaceholderGenerator(IEnumerable<Variable> variables)
        {
            Variables = variables;
            Separator = ",";
        }

        public IEnumerable<Variable> Variables { get; private set; }

        public string Prefix { get; set; }

        public string Separator { get; set; }

        public bool AreReservedCharactersAllowed { get; set; }

        public bool IsQualified { get; set; }

        public bool IsQualifiedWhenEmpty { get; set; }

        public string Generate(object parameters)
        {
            IProperty[] properties = getProperties(parameters);
            var substitutions = from variable in Variables
                                join property in properties on variable.Name equals property.Name
                                from value in getSubstitutions(variable, property)
                                select qualify(variable, value);
            var result = Prefix + String.Join(Separator, substitutions);
            return result;
        }

        private object[] getSubstitutions(Variable variable, IProperty property)
        {
            var values = getParameterValues(variable, property);
            var parts = values.Select(v => escape(variable, v)).ToArray();
            if (variable.IsExploded)
            {
                return parts;
            }
            else
            {
                string combined = String.Join(",", parts);
                return new object[] { combined };
            }
        }

        private object[] getParameterValues(Variable variable, IProperty property)
        {
            if (isSimpleType(property.Type))
            {
                object result = property.Value;
                return new object[] { result };
            }
            else if (isListType(property.Type))
            {
                var result = (IEnumerable)property.Value;
                return result.Cast<object>().ToArray();
            }
            else
            {
                object instance = property.Value;
                var subProperties = getProperties(instance);
                List<object> values = new List<object>();
                foreach (IProperty subProperty in subProperties)
                {
                    object value = subProperty.Value;
                    if (variable.IsExploded)
                    {
                        var pair = new Tuple<string,object>(subProperty.Name, value);
                        values.Add(pair);
                    }
                    else
                    {
                        values.Add(subProperty.Name);
                        values.Add(value);
                    }
                }
                return values.ToArray();
            }
        }

        private static IProperty[] getProperties(object parameters)
        {
            if (parameters == null)
            {
                return new IProperty[0];
            }
            if (isDictionaryType(parameters.GetType()))
            {
                List<IProperty> properties = new List<IProperty>();
                foreach (var pair in (dynamic)parameters)
                {
                    IProperty property = createDictionaryProperty((dynamic)parameters, pair.Key);
                    properties.Add(property);
                }
                return properties.ToArray();
            }
            else
            {
                List<IProperty> properties = new List<IProperty>();
                foreach (PropertyInfo propertyInfo in getReflectedProperties(parameters))
                {
                    IProperty property = new ReflectedProperty(parameters, propertyInfo);
                    properties.Add(property);
                }
                return properties.ToArray();
            }
        }

        private static DictionaryProperty<TValue> createDictionaryProperty<TValue>(IDictionary<string, TValue> dictionary, string key)
        {
            return new DictionaryProperty<TValue>(dictionary, key);
        }

        private static PropertyInfo[] getReflectedProperties(object instance)
        {
            if (instance == null)
            {
                return new PropertyInfo[0];
            }
            Type type = instance.GetType();
            var properties = from property in type.GetProperties()
                             where property.CanRead
                             select property;
            return properties.ToArray();
        }

        private static bool isListType(Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return false;
            }
            if (type.IsGenericType)
            {
                Type[] typeArguments = type.GetGenericArguments();
                if (typeArguments.Length != 1)
                {
                    return false;
                }
                Type typeArgument = typeArguments.Single();
                if (!isSimpleType(typeArgument))
                {
                    return false;
                }
                return true;
            }
            else if (type.IsArray)
            {
                return isSimpleType(type.GetElementType());
            }
            else
            {
                return false;
            }
        }

        private static bool isDictionaryType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            Type[] typeArguments = type.GetGenericArguments();
            if (typeArguments.Length != 2)
            {
                return false;
            }
            Type dictionaryType = typeof(IDictionary<,>).MakeGenericType(typeArguments);
            if (!dictionaryType.IsAssignableFrom(type))
            {
                return false;
            }
            Type keyArgumentType = typeArguments.First();
            return keyArgumentType == typeof(String);
        }
        
        private static bool isSimpleType(Type type)
        {
            Type[] simpleTypes = new Type[]
            { 
                typeof(string), 
                typeof(byte), typeof(byte?),
                typeof(short), typeof(short?),
                typeof(int), typeof(int?),
                typeof(long), typeof(long?),
                typeof(Guid), typeof(Guid?)
            };
            return simpleTypes.Contains(type);
        }

        private string qualify(Variable variable, object value)
        {
            var pair = value as Tuple<string, string>;
            if (pair != null)
            {
                return pair.Item1 + "=" + pair.Item2;
            }
            string stringValue = value as String;
            if (stringValue == null)
            {
                return null;
            }
            if (!IsQualified)
            {
                return stringValue;
            }
            if (!IsQualifiedWhenEmpty && String.IsNullOrWhiteSpace(stringValue))
            {
                return variable.Name;
            }
            return variable.Name + "=" + stringValue;
        }

        private object escape(Variable variable, object value)
        {
            if (value == null)
            {
                return null;
            }
            var pair = value as Tuple<string, object>;
            if (pair != null)
            {
                string pairKey = escape(variable, pair.Item1);
                string unescaped = pair.Item2 == null ? null : pair.Item2.ToString();
                string pairValue = escape(variable, unescaped);
                return new Tuple<string, string>(pairKey, pairValue);
            }
            return escape(variable, value.ToString());
        }

        private string escape(Variable variable, string value)
        {
            if (value == null)
            {
                return null;
            }
            if (variable.MaxLength < value.Length)
            {
                value = value.Substring(0, variable.MaxLength.Value);
            }
            if (AreReservedCharactersAllowed)
            {
                return value.Replace(" ", "+");
            }
            else
            {
                return WebUtility.UrlEncode(value);
            }
        }

        private interface IProperty
        {
            string Name { get; }

            Type Type { get; }

            object Value { get; }
        }

        private class ReflectedProperty : IProperty
        {
            private readonly object instance;
            private PropertyInfo property;

            public ReflectedProperty(object instance, PropertyInfo property)
            {
                this.instance = instance;
                this.property = property;
            }

            public string Name
            {
                get { return property.Name; }
            }

            public Type Type
            {
                get { return property.PropertyType; }
            }

            public object Value
            {
                get { return property.GetValue(instance); }
            }
        }

        private class DictionaryProperty<TValue> : IProperty
        {
            private readonly IDictionary<string, TValue> dictionary;
            private readonly string name;

            public DictionaryProperty(IDictionary<string, TValue> dictionary, string name)
            {
                this.dictionary = dictionary;
                this.name = name;
            }

            public string Name
            {
                get { return name; }
            }

            public Type Type
            {
                get { return typeof(TValue); }
            }

            public object Value
            {
                get { return dictionary.ContainsKey(Name) ? dictionary[Name] : default(TValue); }
            }
        }

    }
}
