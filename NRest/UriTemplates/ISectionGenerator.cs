using System;

namespace NRest.UriTemplates
{
    internal interface ISectionGenerator
    {
        string Generate(object parameters);
    }
}
