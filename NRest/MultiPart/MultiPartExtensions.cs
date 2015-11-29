using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using HttpMultipartParser;

namespace NRest.MultiPart
{
    public static class MultiPartExtensions
    {
        public const string ContentTypePrefix = "multipart/form-data";

        public static IRequestConfiguration WithMultiPartBody(this IRequestConfiguration configuration, Action<IMultiPartBodyBuilder> multiPartBuilder)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (multiPartBuilder == null)
            {
                throw new ArgumentNullException("multiPartBuilder");
            }
            return configuration
                .ConfigureRequest(r => r.ContentType = "multipart/form-data; boundary=" + MultiPartBodyBuilder.Boundary)
                .WithBodyBuilder(new MultiPartBodyBuilder(multiPartBuilder));
        }

        public static MultiPartResponse FromMultiPart(this IWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            // Make sure it is a multi-part request
            string[] parts = response.Response.ContentType.Split(';').Select(s => s.Trim()).ToArray();
            if (!parts[0].Equals(ContentTypePrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return new MultiPartResponse() { Files = new MultiPartFileLookup(), FormData = new NameValueCollection() };
            }
            // Parse the content type parameters
            var contentTypeParameters = parts
                .Skip(1)
                .Select(p => p.Split(new char[] { '=' }, 2))
                .Where(p => p.Length == 2)
                .ToLookup(p => p[0], p => p[1], StringComparer.InvariantCultureIgnoreCase);
            // Check the boundary is specified, and only once
            if (contentTypeParameters["boundary"].Count() != 1)
            {
                return new MultiPartResponse() { Files = new MultiPartFileLookup(), FormData = new NameValueCollection() };
            }
            string boundary = contentTypeParameters["boundary"].First();

            using (Stream responseStream = response.Response.GetResponseStream())
            {
                MultipartFormDataParser parser = new MultipartFormDataParser(responseStream, boundary);

                MultiPartFileLookup fileLookup = new MultiPartFileLookup();
                foreach (var parsedFile in parser.Files)
                {
                    var file = new MultiPartFile()
                    {
                        Name = parsedFile.Name,
                        FileName = parsedFile.FileName,
                        ContentType = parsedFile.ContentType,
                    };
                    file.Contents = copyData(parsedFile.Data);
                    fileLookup.Add(file.Name, file);
                }

                NameValueCollection collection = new NameValueCollection();
                foreach (var parsedParameter in parser.Parameters)
                {
                    collection.Add(parsedParameter.Name, parsedParameter.Data);
                }
                return new MultiPartResponse() { Files = fileLookup, FormData = collection };
            }
        }

        private static byte[] copyData(Stream source)
        {
            MemoryStream destination = new MemoryStream();
            source.CopyTo(destination);
            return destination.ToArray();
        }
    }
}
