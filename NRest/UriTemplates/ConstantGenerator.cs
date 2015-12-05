using System;

namespace NRest.UriTemplates
{
    internal class ConstantGenerator : ISectionGenerator
    {
        public ConstantGenerator(string constant)
        {
            this.Constant = constant;
        }

        public string Constant { get; private set; }

        public string Generate(object parameters)
        {
            return Constant;
        }
    }
}
