using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NRest.ModelBinders
{
    public class NameValueCollectionModelBinder<T> : IModelBinder<T>
    {
        private readonly NameValueCollection collection;

        public NameValueCollectionModelBinder(NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.collection = collection;
        }

        public void Bind(T instance)
        {
            if (instance == null)
            {
                return;
            }
            NameValueCollectionValueProvider provider = new NameValueCollectionValueProvider(collection);
            var properties = getProperties(instance);
            foreach (PropertyInfo property in properties)
            {
                ValueProviderResult result = provider.GetValue(property.PropertyType, property.Name);
                if (result.IsSuccess)
                {
                    property.SetValue(instance, result.Value);
                }
            }
        }

        private static IEnumerable<PropertyInfo> getProperties(T instance)
        {
            Type type = instance.GetType();
            var properties = from property in type.GetProperties()
                             where property.CanWrite
                             where NameValueCollectionValueProvider.IsSupportedType(property.PropertyType)
                             select property;
            return properties.ToArray();
        }
    }
}
