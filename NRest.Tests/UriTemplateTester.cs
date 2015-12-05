using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRest.UriTemplates;

namespace NRest.Tests
{
    [TestClass]
    public class UriTemplateTester
    {
        #region Level 1

        [TestMethod]
        public void ShouldExpandVariable()
        {
            UriTemplate template = new UriTemplate("{var}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithReservedCharacter()
        {
            UriTemplate template = new UriTemplate("{hello}");
            string result = template.Expand(new { hello = "Hello World!" });
            Assert.AreEqual("Hello+World!", result);
        }

        #endregion

        #region Level 2

        [TestMethod]
        public void ShouldExpandVariableWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+var}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSpaceWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+hello}");
            string result = template.Expand(new { hello = "Hello, World!" });
            Assert.AreEqual("Hello,+World!", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSlashWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+path}/here");
            string result = template.Expand(new { path = "/foo/bar" });
            Assert.AreEqual("/foo/bar/here", result);
        }

        [TestMethod]
        public void ShouldExpandVariableInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("X{#var}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("X#value", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithReservedCharacterInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("X{#hello}");
            string result = template.Expand(new { hello = "Hello World!" });
            Assert.AreEqual("X#Hello+World!", result);
        }

        #endregion

        #region Level 3

        [TestMethod]
        public void ShouldExpandVariables()
        {
            UriTemplate template = new UriTemplate("map?{x,y}");
            string result = template.Expand(new { x = "1024", y = "768" });
            Assert.AreEqual("map?1024,768", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithReservedCharacters()
        {
            UriTemplate template = new UriTemplate("{x,hello,y}");
            string result = template.Expand(new { x = "1024", hello = "Hello World!", y = 768 });
            Assert.AreEqual("1024,Hello+World!,768", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithSpaceWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+x,hello,y}");
            string result = template.Expand(new { x = 1024, hello = "Hello World!", y = 768 });
            Assert.AreEqual("1024,Hello+World!,768", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithSlashWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+path,x}/here");
            string result = template.Expand(new { x = 1024, path = "/foo/bar", y = 768 });
            Assert.AreEqual("/foo/bar,1024/here", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithSpaceWhenReservedAllowedInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#x,hello,y}");
            string result = template.Expand(new { x = 1024, hello = "Hello World!", y = 768 });
            Assert.AreEqual("#1024,Hello+World!,768", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithSlashWhenReservedAllowedInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#path,x}/here");
            string result = template.Expand(new { x = 1024, path = "/foo/bar", y = 768 });
            Assert.AreEqual("#/foo/bar,1024/here", result);
        }

        [TestMethod]
        public void ShouldExpandVariableInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.var}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("X.value", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.x,y}");
            string result = template.Expand(new { x = 1024, y = 768 });
            Assert.AreEqual("X.1024.768", result);
        }

        [TestMethod]
        public void ShouldExpandVariableInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/var}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("/value", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/var,x}/here");
            string result = template.Expand(new { var = "value", x = 1024 });
            Assert.AreEqual("/value/1024/here", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;x,y}");
            string result = template.Expand(new { x = 1024, y = 768 });
            Assert.AreEqual(";x=1024;y=768", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithEmptyForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;x,y,empty}");
            string result = template.Expand(new { x = 1024, y = 768, empty = "" });
            Assert.AreEqual(";x=1024;y=768;empty", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesInQuery()
        {
            UriTemplate template = new UriTemplate("{?x,y}");
            string result = template.Expand(new { x = 1024, y = 768 });
            Assert.AreEqual("?x=1024&y=768", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithEmptyInQuery()
        {
            UriTemplate template = new UriTemplate("{?x,y,empty}");
            string result = template.Expand(new { x = 1024, y = 768, empty = "" });
            Assert.AreEqual("?x=1024&y=768&empty=", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("?fixed=yes{&x}");
            string result = template.Expand(new { x = 1024 });
            Assert.AreEqual("?fixed=yes&x=1024", result);
        }

        [TestMethod]
        public void ShouldExpandVariablesWithEmptyInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&x,y,empty}");
            string result = template.Expand(new { x = 1024, y = 768, empty = "" });
            Assert.AreEqual("&x=1024&y=768&empty=", result);
        }

        #endregion

        #region Level 4

        [TestMethod]
        public void ShouldExpandVariableWithSizeLimitSmallerThanValue()
        {
            UriTemplate template = new UriTemplate("{var:3}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("val", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSizeLimitGreaterThanValue()
        {
            UriTemplate template = new UriTemplate("{var:30}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void ShouldExpandList()
        {
            UriTemplate template = new UriTemplate("{list}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExploded()
        {
            UriTemplate template = new UriTemplate("{list*}");
            string result = template.Expand(new { list = new List<string>() { "red", "green", "blue" } });
            Assert.AreEqual("red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandPairs()
        {
            UriTemplate template = new UriTemplate("{keys}");
            string result = template.Expand(new 
            { 
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionary()
        {
            UriTemplate template = new UriTemplate("{keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExploded()
        {
            UriTemplate template = new UriTemplate("{keys*}");
            string result = template.Expand(new
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("semi=%3B,dot=.,comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExploded()
        {
            UriTemplate template = new UriTemplate("{keys*}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("semi=%3B,dot=.,comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSmallerSizeLimitWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+path:6}/here");
            string result = template.Expand(new { path = "/foo/bar" });
            Assert.AreEqual("/foo/b/here", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+list}");
            string result = template.Expand(new { list = new HashSet<string>() { "red", "green", "blue" } });
            Assert.AreEqual("red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExplodedWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+list*}");
            string result = template.Expand(new { list = new List<string>() { "red", "green", "blue" } });
            Assert.AreEqual("red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+keys}");
            string result = template.Expand(new
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("semi,;,dot,.,comma,,", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("semi,;,dot,.,comma,,", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExplodedWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+keys*}");
            string result = template.Expand(new
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("semi=;,dot=.,comma=,", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExplodedWhenReservedAllowed()
        {
            UriTemplate template = new UriTemplate("{+keys*}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("semi=;,dot=.,comma=,", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSmallerSizeLimitInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#path:6}/here");
            string result = template.Expand(new { path = "/foo/bar" });
            Assert.AreEqual("#/foo/b/here", result);
        }

        [TestMethod]
        public void ShouldExpandListInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#list}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("#red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExplodedInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#list*}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("#red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#keys}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("#semi,;,dot,.,comma,,", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("#semi,;,dot,.,comma,,", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExplodedInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#keys*}");
            string result = template.Expand(new
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("#semi=;,dot=.,comma=,", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExplodedInFragmentExpansion()
        {
            UriTemplate template = new UriTemplate("{#keys*}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("#semi=;,dot=.,comma=,", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSmallerSizeLimitInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.var:3}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("X.val", result);
        }

        [TestMethod]
        public void ShouldExpandListInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.list}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("X.red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExplodedInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.list*}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("X.red.green.blue", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.keys}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("X.semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("X.semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExplodedInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.keys*}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("X.semi=%3B.dot=..comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExplodedInLabelExpansion()
        {
            UriTemplate template = new UriTemplate("X{.keys*}");
            string result = template.Expand(new 
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("X.semi=%3B.dot=..comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSizeLimitSmallerThanValueInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/var:1,var}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("/v/value", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExplodedInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/list}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("/red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListAndVariableWithSizeLimiteInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/list*,path:4}");
            string result = template.Expand(new 
            { 
                list = new string[] { "red", "green", "blue" },
                path = "/foo/bar"
            });
            Assert.AreEqual("/red/green/blue/%2Ffoo", result);
        }

        [TestMethod]
        public void ShouldExpandPairsInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/keys}");
            string result = template.Expand(new
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("/semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("/semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExplodedInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/keys*}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("/semi=%3B/dot=./comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExplodedInPathSegment()
        {
            UriTemplate template = new UriTemplate("{/keys*}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("/semi=%3B/dot=./comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSizeLimitSmallerThanValueForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;hello:5}");
            string result = template.Expand(new { hello = "Hello World!" });
            Assert.AreEqual(";hello=Hello", result);
        }

        [TestMethod]
        public void ShouldExpandListForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;list}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual(";list=red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExplodedForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;list*}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual(";list=red;list=green;list=blue", result);
        }

        [TestMethod]
        public void ShouldExpandPairsForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;keys}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual(";keys=semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual(";keys=semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExplodedForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;keys*}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual(";semi=%3B;dot=.;comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExplodedForPathStyleParameters()
        {
            UriTemplate template = new UriTemplate("{;keys*}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual(";semi=%3B;dot=.;comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSizeLimitSmallerThanValueInQuery()
        {
            UriTemplate template = new UriTemplate("{?var:3}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("?var=val", result);
        }

        [TestMethod]
        public void ShouldExpandListInQuery()
        {
            UriTemplate template = new UriTemplate("{?list}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("?list=red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExplodedInQuery()
        {
            UriTemplate template = new UriTemplate("{?list*}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("?list=red&list=green&list=blue", result);
        }

        [TestMethod]
        public void ShouldExpandPairsInQuery()
        {
            UriTemplate template = new UriTemplate("{?keys}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("?keys=semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryInQuery()
        {
            UriTemplate template = new UriTemplate("{?keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("?keys=semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExplodedInQuery()
        {
            UriTemplate template = new UriTemplate("{?keys*}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("?semi=%3B&dot=.&comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExplodedInQuery()
        {
            UriTemplate template = new UriTemplate("{?keys*}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("?semi=%3B&dot=.&comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandVariableWithSizeLimitSmallerThanValueInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&var:3}");
            string result = template.Expand(new { var = "value" });
            Assert.AreEqual("&var=val", result);
        }

        [TestMethod]
        public void ShouldExpandListInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&list}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("&list=red,green,blue", result);
        }

        [TestMethod]
        public void ShouldExpandListWhenExplodedInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&list*}");
            string result = template.Expand(new { list = new string[] { "red", "green", "blue" } });
            Assert.AreEqual("&list=red&list=green&list=blue", result);
        }

        [TestMethod]
        public void ShouldExpandPairsInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&keys}");
            string result = template.Expand(new 
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("&keys=semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&keys}");
            string result = template.Expand(new
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("&keys=semi,%3B,dot,.,comma,%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsWhenExplodedInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&keys*}");
            string result = template.Expand(new
            {
                keys = new
                {
                    semi = ";",
                    dot = ".",
                    comma = ","
                }
            });
            Assert.AreEqual("&semi=%3B&dot=.&comma=%2C", result);
        }

        [TestMethod]
        public void ShouldExpandPairsAsDictionaryWhenExplodedInQueryContinuation()
        {
            UriTemplate template = new UriTemplate("{&keys*}");
            string result = template.Expand(new 
            {
                keys = new Dictionary<string, string>()
                {
                    { "semi", ";" },
                    { "dot", "." },
                    { "comma", "," }
                }
            });
            Assert.AreEqual("&semi=%3B&dot=.&comma=%2C", result);
        }

        #endregion

        #region Examples

        [TestMethod]
        public void ShouldHandleMultipleSubstitutions1()
        {
            UriTemplate template = new UriTemplate("http://localhost{+port}/api{/version}/customers{?q,pagenum,pagesize}{#section}");
            string uri = template.Expand(new
            {
                port = ":8080",
                version = "v2",
                q = "rest",
                pagenum = 3,
                pagesize = (int?)null,
                section = "results"
            });
            Assert.AreEqual("http://localhost:8080/api/v2/customers?q=rest&pagenum=3&pagesize=#results", uri);
        }

        #endregion
    }
}
