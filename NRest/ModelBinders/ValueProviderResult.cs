using System;

namespace NRest.ModelBinders
{
    public class ValueProviderResult
    {
        public bool IsSuccess { get; set; }

        public string RawValue { get; set; }

        public object Value { get; set; }
    }
}
