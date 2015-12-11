using System;
using System.Collections.Generic;

namespace NRest.Reflection
{
    internal interface IPropertyLookup
    {
        IProperty GetProperty(string name);

        IEnumerable<IProperty> GetProperties();
    }
}
