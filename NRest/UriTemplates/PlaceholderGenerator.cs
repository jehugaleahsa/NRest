using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using NRest.Reflection;

namespace NRest.UriTemplates
{
    internal class PlaceholderGenerator : ISectionGenerator
    {
        public PlaceholderGenerator(IEnumerable<Variable> variables)
        {
            Variables = variables;
            Separator = ",";
        }

        public IEnumerable<Variable> Variables { get; private set; }

        public string Prefix { get; set; }

        public string Separator { get; set; }

        public bool AreReservedCharactersAllowed { get; set; }

        public bool IsQualified { get; set; }

        public bool IsQualifiedWhenEmpty { get; set; }

        public string Generate(object parameters)
        {
            IPropertyLookup properties = PropertyLookup.CreatePropertyLookup(parameters);
            var substitutions = from variable in Variables
                                let property = properties.GetProperty(variable.Name)
                                where property != null
                                from value in getSubstitutions(variable, property)
                                select qualify(variable, value);
            var result = Prefix + String.Join(Separator, substitutions);
            return result;
        }

        private object[] getSubstitutions(Variable variable, IProperty property)
        {
            var values = getParameterValues(variable, property);
            var parts = values.Select(v => escape(variable, v)).ToArray();
            if (variable.IsExploded)
            {
                return parts;
            }
            else
            {
                string combined = String.Join(",", parts);
                return new object[] { combined };
            }
        }

        private IEnumerable<object> getParameterValues(Variable variable, IProperty property)
        {
            return PropertyLookup.GetValues(property, variable.IsExploded);
        }

        private string qualify(Variable variable, object value)
        {
            var pair = value as Tuple<string, string>;
            if (pair != null)
            {
                return pair.Item1 + "=" + pair.Item2;
            }
            string stringValue = value as String;
            if (stringValue == null)
            {
                return null;
            }
            if (!IsQualified)
            {
                return stringValue;
            }
            if (!IsQualifiedWhenEmpty && String.IsNullOrWhiteSpace(stringValue))
            {
                return variable.Name;
            }
            return variable.Name + "=" + stringValue;
        }

        private object escape(Variable variable, object value)
        {
            if (value == null)
            {
                return null;
            }
            var pair = value as Tuple<string, object>;
            if (pair != null)
            {
                string pairKey = escape(variable, pair.Item1);
                string unescaped = pair.Item2 == null ? null : pair.Item2.ToString();
                string pairValue = escape(variable, unescaped);
                return new Tuple<string, string>(pairKey, pairValue);
            }
            return escape(variable, value.ToString());
        }

        private string escape(Variable variable, string value)
        {
            if (value == null)
            {
                return null;
            }
            if (variable.MaxLength < value.Length)
            {
                value = value.Substring(0, variable.MaxLength.Value);
            }
            if (AreReservedCharactersAllowed)
            {
                return value.Replace(" ", "+");
            }
            else
            {
                return WebUtility.UrlEncode(value);
            }
        }
    }
}
