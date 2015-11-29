using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRest.MultiPart;

namespace NRest.ModelBinders
{
    public class MultiPartModelBinder<T> : IModelBinder<T>
    {
        private readonly NameValueCollectionModelBinder<T> formBinder;
        private readonly MultiPartFileLookup fileLookup;

        public MultiPartModelBinder(MultiPartResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            this.formBinder = new NameValueCollectionModelBinder<T>(response.FormData);
            this.fileLookup = response.Files;
        }

        public void Bind(T instance)
        {
            if (instance == null)
            {
                return;
            }
            formBinder.Bind(instance);

            var fileProperties = getFileProperties(instance);
            foreach (PropertyInfo property in fileProperties)
            {
                if (fileLookup.HasName(property.Name))
                {
                    MultiPartFile file = fileLookup.GetFiles(property.Name).First();
                    property.SetValue(instance, file.Contents);
                }
            }
        }

        private IEnumerable<PropertyInfo> getFileProperties(T instance)
        {
            Type type = instance.GetType();
            var properties = from property in type.GetProperties()
                             where property.CanWrite
                             where property.PropertyType == typeof(byte[])
                             select property;
            return properties.ToArray();
        }
    }
}
