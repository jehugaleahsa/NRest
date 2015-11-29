using System;
using System.Collections.Generic;

namespace NRest.ModelBinders
{
    public class CompoundModelBinder<T> : IModelBinder<T>
    {
        private readonly List<IModelBinder<T>> binders;

        public CompoundModelBinder()
        {
            this.binders = new List<IModelBinder<T>>();
        }

        public void AddBinder(IModelBinder<T> binder)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }
            binders.Add(binder);
        }

        public void Bind(T instance)
        {
            foreach (IModelBinder<T> binder in binders)
            {
                binder.Bind(instance);
            }
        }
    }
}
