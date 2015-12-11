using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NRest.Reflection
{
    internal class ObjectPropertyLookup : IPropertyLookup
    {
        private readonly Dictionary<string, ReflectedProperty> propertyLookup;

        public ObjectPropertyLookup(object instance)
        {
            this.propertyLookup = getReflectedProperties(instance);
        }

        private static Dictionary<string, ReflectedProperty> getReflectedProperties(object instance)
        {
            if (instance == null)
            {
                return new Dictionary<string, ReflectedProperty>();
            }
            Type type = instance.GetType();
            var properties = from property in type.GetProperties()
                             where property.CanRead
                             select new ReflectedProperty(instance, property);
            return properties.ToDictionary(p => p.Name);
        }

        public IProperty GetProperty(string name)
        {
            ReflectedProperty property;
            if (propertyLookup.TryGetValue(name, out property))
            {
                return property;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<IProperty> GetProperties()
        {
            return propertyLookup.Values;
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
    }
}
