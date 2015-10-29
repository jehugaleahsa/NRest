using System;
using System.Net;
using FakeServers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NRest.Tests
{
    [TestClass]
    public class RestClientTester
    {
        [TestMethod]
        public void ShouldGET()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                server.StatusCode = HttpStatusCode.OK;

                server.Listen();
                
                RestClient client = new RestClient();
                var response = client.Get("http://localhost:8080/api/customers").Execute();

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The wrong status code was returned.");
                Assert.IsFalse(response.HasError, "A success code was returned. There should be no error.");
                Assert.IsNull(response.Result, "No WHEN handler was defined. The result should be null.");
            }
        }

        [TestMethod]
        public void ShouldGETWithCustomHeader()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var extractor = new HeaderExtractor();
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Get("http://localhost:8080/api/customers")
                    .WithHeader("test_header", "test_value")
                    .Execute();

                var value = extractor.Headers.Get("test_header");
                Assert.AreEqual("test_value", value, "The header was not passed to the server.");
            }
        }
    }
}
