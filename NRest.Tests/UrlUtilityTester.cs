using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NRest.Tests
{
    [TestClass]
    public class UrlUtilityTester
    {
        /// <summary>
        /// This test verifies that even though Microsoft doesn't encode
        /// spaces and other punctuation exactly to the specification,
        /// it still decodes correctly.
        /// </summary>
        [TestMethod]
        public void ShouldDecodeSpace()
        {
            string url = "Hello%20World%21";
            string decoded = WebUtility.UrlDecode(url);
            Assert.AreEqual("Hello World!", decoded);
        }
    }
}
