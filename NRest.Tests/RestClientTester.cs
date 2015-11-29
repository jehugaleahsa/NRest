using System.Net;
using FakeServers;
using FakeServers.Extractors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRest.Forms;

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
                var extractor = new RequestExtractor();
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

        [TestMethod]
        public void ShouldPOSTWithFormData()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var bodyExtractor = new UrlEncodedBodyExtractor();
                var extractor = new RequestExtractor(bodyExtractor);
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Post("http://localhost:8080/api/customers")
                    .WithUrlEncodedBody(b => b
                        .WithParameter("Name", "Bob Smith")
                        .WithParameter("Age", 31)
                        .WithParameter("Title", "Mr."))
                    .Execute();

                string name = bodyExtractor.Parameters["Name"];
                string age = bodyExtractor.Parameters["Age"];
                string title = bodyExtractor.Parameters["Title"];

                Assert.AreEqual("Bob Smith", name, "The name was not sent.");
                Assert.AreEqual("31", age, "The age was not sent.");
                Assert.AreEqual("Mr.", title, "The title was not sent.");
            }
        }

        [TestMethod]
        public void ShouldPOSTWithArrayFormData()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var bodyExtractor = new UrlEncodedBodyExtractor();
                var extractor = new RequestExtractor(bodyExtractor);
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Post("http://localhost:8080/api/customers")
                    .WithUrlEncodedBody(b => b
                        .WithParameter("CustomerId", 1)
                        .WithParameter("CustomerId", 2)
                        .WithParameter("CustomerId", 3))
                    .Execute();

                string[] ids = bodyExtractor.Parameters.GetValues("CustomerId");
                string[] expectedIds = new string[] { "1", "2", "3" };
                CollectionAssert.AreEquivalent(expectedIds, ids, "The array of values were not sent.");
            }
        }
    }
}
