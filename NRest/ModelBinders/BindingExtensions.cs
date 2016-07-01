using System;

namespace NRest.ModelBinders
{
    public static class BindingExtensions
    {
        public static T BindTo<T>(this IWebResponse response, IModelBinder<T> binder = null)
            where T : new()
        {
            if (binder == null)
            {
                binder = new ModelBinder<T>(response);
            }
            T instance = new T();
            binder.Bind(instance);
            return instance;
        }

        public static T BindTo<T>(this IWebResponse response, T instance, IModelBinder<T> binder = null)
        {
            if (binder == null)
            {
                binder = new ModelBinder<T>(response);
            }
            binder.Bind(instance);
            return instance;
        }
    }
}
