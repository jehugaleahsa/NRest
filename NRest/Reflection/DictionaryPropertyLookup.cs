using System;
using System.Collections.Generic;
using System.Linq;

namespace NRest.Reflection
{
    internal static class DictionaryPropertyLookup
    {
        public static DictionaryPropertyLookup<TValue> Create<TValue>(IDictionary<string, TValue> dictionary)
        {
            return new DictionaryPropertyLookup<TValue>(dictionary);
        }
    }

    internal class DictionaryPropertyLookup<TValue> : IPropertyLookup
    {
        private readonly IDictionary<string, TValue> dictionary;

        public DictionaryPropertyLookup(IDictionary<string, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        public IProperty GetProperty(string name)
        {
            if (dictionary.ContainsKey(name))
            {
                return new DictionaryProperty(dictionary, name);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<IProperty> GetProperties()
        {
            return dictionary.Keys.Select(k => new DictionaryProperty(dictionary, k));
        }

        private class DictionaryProperty : IProperty
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
