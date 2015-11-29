using System;

namespace NRest.ModelBinders
{
    public interface IValueProvider
    {
        ValueProviderResult GetValue(Type type, string key);
    }
}
