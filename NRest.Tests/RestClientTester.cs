using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FakeServers;
using FakeServers.Extractors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRest.Forms;
using NRest.Json;
using NRest.MultiPart;
using NRest.Primitives;

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
                Assert.IsTrue(response.IsSuccessStatusCode, "A success code was returned. There should be no error.");
                Assert.IsNull(response.Result, "No WHEN handler was defined. The result should be null.");
            }
        }

        [TestMethod]
        public void ShouldGETWithSimpleTemplate()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var extractor = new RequestExtractor();
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Get("http://localhost:8080/api/customers/{customerId}", new 
                    {
                        customerId = 123
                    }).Execute();

                Assert.IsTrue(extractor.Url.ToString().EndsWith("123"), "The ID was not passed.");
            }
        }

        [TestMethod]
        public void ShouldGETWithQueryTemplate()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var extractor = new RequestExtractor();
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Get("http://localhost:8080/api/customers/{?customerid}", new
                {
                    customerid = 123
                }).Execute();

                Assert.AreEqual("123", extractor.QueryString["customerid"], "The ID was not passed.");
            }
        }

        [TestMethod]
        public void ShouldGETWithQueryParametersObject()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var extractor = new RequestExtractor();
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Get("http://localhost:8080/api/customers")
                    .WithQueryParameters(new { customerid = 123 })
                    .Execute();

                Assert.AreEqual("123", extractor.QueryString["customerid"], "The ID was not passed.");
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
        public void ShouldGETWithCustomHeadersObject()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var extractor = new RequestExtractor();
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Get("http://localhost:8080/api/customers")
                    .WithHeaders(new { test_header = "test_value" })
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
        public void ShouldPOSTWithFormDataObject()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var bodyExtractor = new UrlEncodedBodyExtractor();
                var extractor = new RequestExtractor(bodyExtractor);
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Post("http://localhost:8080/api/customers")
                    .WithUrlEncodedBody(new
                    {
                        Name = "Bob Smith",
                        Age = 31,
                        Title = "Mr."
                    })
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

        [TestMethod]
        public void ShouldPOSTWithJsonData()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                var bodyExtractor = new JsonBodyExtractor<TestCustomer>();
                server.UseBodyExtractor(bodyExtractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Post("http://localhost:8080/api/customers")
                    .WithJsonBody(new TestCustomer() { Name = "Bob Smith", Age = 31, Title = "Mr." })
                    .Execute();

                var customer = bodyExtractor.Result;
                Assert.AreEqual("Bob Smith", customer.Name, "The name was not sent.");
                Assert.AreEqual(31, customer.Age, "The age was not sent.");
                Assert.AreEqual("Mr.", customer.Title, "The title was not sent.");
            }
        }

        [TestMethod]
        public void ShouldPOSTWithNoBody()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                RequestExtractor extractor = new RequestExtractor();
                server.UseBodyExtractor(extractor);
                server.Listen();

                RestClient client = new RestClient();
                var response = client.Post("http://localhost:8080/api/customers")
                    .WhenError(r => { throw new Exception(r.FromString<string>()); })
                    .Execute();

                string contentLength = extractor.Headers["Content-Length"];
                Assert.AreEqual("0", contentLength, "The content length was not specified.");
            }
        }

        public class TestCustomer
        {
            public string Name { get; set; }

            public int? Age { get; set; }

            public string Title { get; set; }
        }

        [TestMethod]
        public void ShouldPOSTMultiPartData()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                MultiPartBodyExtractor extractor = new MultiPartBodyExtractor();
                server.UseBodyExtractor(extractor);

                server.Listen();

                RestClient client = new RestClient("http://localhost:8080");
                client.Post("api/customers")
                    .WithMultiPartBody(b =>
                    {
                        b.WithFormData(ub => ub.WithParameter("name", "John Smith"));
                        b.WithFile("file1", "path", Encoding.UTF8.GetBytes("Hello, World!Ӽ!"), "text/plain");
                    })
                    .Execute();

                Assert.AreEqual("John Smith", extractor.Parameters["name"], "The form data was not transfered.");
                
                var file = extractor.Files.GetFiles("file1").SingleOrDefault();
                Assert.AreEqual("file1", file.Name);
                Assert.AreEqual("path", file.FileName);
                Assert.AreEqual("text/plain", file.ContentType);
                Assert.AreEqual("Hello, World!Ӽ!", Encoding.UTF8.GetString(file.Contents));
            }
        }

        [TestMethod]
        public async Task ShouldPOSTMultiPartDataAsync()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                MultiPartBodyExtractor extractor = new MultiPartBodyExtractor();
                server.UseBodyExtractor(extractor);

                server.Listen();

                RestClient client = new RestClient("http://localhost:8080");
                await client.Post("api/customers")
                    .WithMultiPartBody(b =>
                    {
                        b.WithFormData(ub => ub.WithParameter("name", "John Smith"));
                        b.WithFile("file1", "path", Encoding.UTF8.GetBytes("Hello, world"), "text/plain");
                    })
                    .ExecuteAsync();

                Assert.AreEqual("John Smith", extractor.Parameters["name"], "The form data was not transfered.");

                var file = extractor.Files.GetFiles("file1").SingleOrDefault();
                Assert.AreEqual("file1", file.Name);
                Assert.AreEqual("path", file.FileName);
                Assert.AreEqual("text/plain", file.ContentType);
                Assert.AreEqual("Hello, world", Encoding.UTF8.GetString(file.Contents));
            }
        }

        [TestMethod]
        public void ShouldGetInt32()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/numbers"))
            {
                server.ReturnString("4");
                server.Listen();

                RestClient client = new RestClient("http://localhost:8080");
                var response = client.Get("numbers")
                    .WhenSuccess(r => r.FromString<int>())
                    .Execute();
                Assert.IsTrue(response.IsSuccessStatusCode, "An error occurred getting the number.");
                Assert.AreEqual(4, response.GetResult<int>());
            }
        }

        [TestMethod]
        public void ShouldPostMultiPartWithQuotedName()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                MultiPartBodyExtractor extractor = new MultiPartBodyExtractor();
                server.UseBodyExtractor(extractor);

                server.Listen();

                RestClient client = new RestClient("http://localhost:8080");
                client.Post("api/customers")
                    .WithMultiPartBody(b =>
                    {
                        b.WithFormData(ub => ub.WithParameter("na\"me", "John Smith"));
                    })
                    .Execute();

                Assert.AreEqual("John Smith", extractor.Parameters["na_me"], "The form data was not transfered.");
            }
        }

        [TestMethod]
        public void ShouldPostMultuPartWithInvalidCharacter()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                MultiPartBodyExtractor extractor = new MultiPartBodyExtractor();
                server.UseBodyExtractor(extractor);

                server.Listen();

                RestClient client = new RestClient("http://localhost:8080");
                client.Post("api/customers")
                    .WithMultiPartBody(b =>
                    {
                        b.WithFormData(ub => ub.WithParameter("naӼme", "John Smith"));
                    })
                    .Execute();

                Assert.AreEqual("John Smith", extractor.Parameters["na?me"], "The form data was not transfered.");
            }
        }

        [TestMethod]
        public void ShouldPostMultuPartWithBase64Content()
        {
            using (FakeHttpServer server = new FakeHttpServer("http://localhost:8080/api/customers"))
            {
                MultiPartBodyExtractor extractor = new MultiPartBodyExtractor();
                server.UseBodyExtractor(extractor);

                server.Listen();

                string message = "Hello, World!Ӽ!";
                byte[] messageRaw = Encoding.UTF8.GetBytes(message);
                string base64 = Convert.ToBase64String(messageRaw);
                byte[] base64Raw = Encoding.UTF8.GetBytes(base64);
                NameValueCollection headers = new NameValueCollection();
                headers.Add("Content-Transfer-Encoding", "base64");

                RestClient client = new RestClient("http://localhost:8080");
                client.Post("api/customers")
                    .WithMultiPartBody(b =>
                    {
                        b.WithFile("file", "file.txt", base64Raw, "text/plain", headers);
                    })
                    .Execute();

                var file = extractor.Files.GetFiles("file").Single();
                CollectionAssert.AreEqual(base64Raw, file.Contents);
            }
        }
    }
}
