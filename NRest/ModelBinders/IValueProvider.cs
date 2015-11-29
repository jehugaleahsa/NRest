using System;
using System.Globalization;

namespace NRest.ModelBinders
{
    public interface IValueProvider
    {
        ValueProviderResult GetValue(Type type, string key);

        ValueProviderResult GetValue(Type type, string key, CultureInfo cultureInfo);
    }
}
