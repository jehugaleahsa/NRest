using System;
using NRest.Forms;
using NRest.Json;
using NRest.MultiPart;
using NRest.Primitives;

namespace NRest.ModelBinders
{
    public class ModelBinder<T> : IModelBinder<T>
    {
        private readonly CompoundModelBinder<T> binder;

        public ModelBinder(IWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            this.binder = getCompoundModelBinder(response);
        }

        private static CompoundModelBinder<T> getCompoundModelBinder(IWebResponse response)
        {
            // Extract property values from headers and the content
            CompoundModelBinder<T> binder = new CompoundModelBinder<T>();
            binder.AddBinder(new NameValueCollectionModelBinder<T>(response.Response.Headers));
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
            if (String.Compare(response.Response.ContentType, JsonExtensions.ContentType, comparison) == 0)
            {
                binder.AddBinder(new JsonModelBinder<T>(response.FromString<string>()));
            }
            else if (String.Compare(response.Response.ContentType, FormExtensions.ContentType, comparison) == 0)
            {
                binder.AddBinder(new NameValueCollectionModelBinder<T>(response.FromForm()));
            }
            else if (response.Response.ContentType.StartsWith(MultiPartExtensions.ContentTypePrefix, comparison))
            {
                binder.AddBinder(new MultiPartModelBinder<T>(response.FromMultiPart()));
            }
            return binder;
        }

        public void Bind(T instance)
        {
            binder.Bind(instance);
        }
    }
}
