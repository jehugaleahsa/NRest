using System;
using NRest.Forms;
using NRest.Json;
using NRest.MultiPart;
using NRest.Primitives;

namespace NRest.ModelBinders
{
    public class ModelBinder<T> : IModelBinder<T>
    {
        private readonly IWebResponse response;

        public ModelBinder(IWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            this.response = response;
        }

        public void Bind(T instance)
        {
            // Extract property values from headers and the content
            CompoundModelBinder<T> binder = new CompoundModelBinder<T>();
            binder.AddBinder(new NameValueCollectionModelBinder<T>(response.Response.Headers));
            if (response.Response.ContentType == JsonExtensions.ContentType)
            {
                binder.AddBinder(new JsonModelBinder<T>(response.FromString<string>()));
            }
            else if (response.Response.ContentType == FormExtensions.ContentType)
            {
                binder.AddBinder(new NameValueCollectionModelBinder<T>(response.FromForm()));
            }
            else if (response.Response.ContentType.StartsWith(MultiPartExtensions.ContentTypePrefix))
            {
                binder.AddBinder(new MultiPartModelBinder<T>(response.FromMultiPart()));
            }
        }
    }
}
