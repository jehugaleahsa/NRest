using System;

namespace NRest.Reflection
{
    internal interface IProperty
    {
        string Name { get; }

        Type Type { get; }

        object Value { get; }
    }
}
