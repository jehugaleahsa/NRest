using System;

namespace NRest.ModelBinders
{
    public interface IModelBinder<T>
    {
        void Bind(T instance);
    }
}
