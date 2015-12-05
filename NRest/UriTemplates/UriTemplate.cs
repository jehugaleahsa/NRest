using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NRest.UriTemplates
{
    public class UriTemplate
    {
        private static readonly Regex parser;
        private static readonly Regex iParser;
        private readonly string template;
        private readonly List<ISectionGenerator> generators;

        static UriTemplate()
        {
            const string prefix = @"(?<prefix>[+#./;?&])?";
            const string variable = @"(?<var>.+?(?::\d+)?\*?)";
            const string variableList = variable + @"(?:," + variable + @")*";
            const string expression = @"{" + prefix + variableList + @"}";
            parser = new Regex(expression);
            iParser = new Regex(expression, RegexOptions.IgnoreCase);
        }

        public UriTemplate(string template, bool caseSensitive = true)
        {
            this.template = template;
            Regex regex = caseSensitive ? parser : iParser;
            this.generators = getGenerators(template, regex);
        }

        private static List<ISectionGenerator> getGenerators(string template, Regex regex)
        {
            List<ISectionGenerator> generators = new List<ISectionGenerator>();
            int startAt = 0;
            var matches = regex.Matches(template);
            foreach (Match match in matches)
            {
                string leadingText = template.Substring(startAt, match.Index - startAt);
                if (leadingText.Length > 0)
                {
                    generators.Add(new ConstantGenerator(leadingText));
                }
                var variables = getVariables(match);
                var prefix = getPrefix(match);
                var generator = getPlaceholderGenerator(prefix, variables);
                generators.Add(generator);
                startAt = match.Index + match.Length;
            }
            string trailingText = template.Substring(startAt);
            if (trailingText.Length > 0)
            {
                generators.Add(new ConstantGenerator(trailingText));
            }
            return generators;
        }        

        private static Variable[] getVariables(Match match)
        {
            var variableGroup = match.Groups["var"];
            var variables = variableGroup.Captures.Cast<Capture>().Select(c => getVariable(c.Value)).ToArray();
            return variables;
        }

        private static Variable getVariable(string capture)
        {
            bool isExploded = false;
            if (capture.EndsWith("*"))
            {
                isExploded = true;
                capture = capture.Substring(0, capture.Length - 1);
            }
            int colonIndex = capture.LastIndexOf(':');
            if (colonIndex == -1)
            {
                return new Variable() { Name = capture, IsExploded = isExploded };
            }
            string lengthString = capture.Substring(colonIndex + 1);
            int length;
            if (!Int32.TryParse(lengthString, out length))
            {
                return new Variable() { Name = capture, IsExploded = isExploded };
            }
            string name = capture.Substring(0, colonIndex);
            return new Variable() { Name = name, MaxLength = length, IsExploded = isExploded };
        }

        private static string getPrefix(Match match)
        {
            var prefix = match.Groups["prefix"].Value;
            return prefix;
        }

        private static PlaceholderGenerator getPlaceholderGenerator(string prefix, Variable[] variables)
        {
            if (prefix == "+")
            {
                return new PlaceholderGenerator(variables) { AreReservedCharactersAllowed = true };
            }
            else if (prefix == "#")
            {
                return new PlaceholderGenerator(variables) { Prefix = "#", AreReservedCharactersAllowed = true };
            }
            else if (prefix == ".")
            {
                return new PlaceholderGenerator(variables) { Prefix = ".", Separator = "." };
            }
            else if (prefix == "/")
            {
                return new PlaceholderGenerator(variables) { Prefix = "/", Separator = "/" };
            }
            else if (prefix == ";")
            {
                return new PlaceholderGenerator(variables) { Prefix = ";", Separator = ";", IsQualified = true };
            }
            else if (prefix == "?")
            {
                return new PlaceholderGenerator(variables) { Prefix = "?", Separator = "&", IsQualified = true, IsQualifiedWhenEmpty = true };
            }
            else if (prefix == "&")
            {
                return new PlaceholderGenerator(variables) { Prefix = "&", Separator = "&", IsQualified = true, IsQualifiedWhenEmpty = true };
            }
            else
            {
                return new PlaceholderGenerator(variables);
            }
        }

        public string Expand(object parameters)
        {
            if (template == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            foreach (ISectionGenerator generator in generators)
            {
                builder.Append(generator.Generate(parameters));
            }
            return builder.ToString();
        }
    }
}
