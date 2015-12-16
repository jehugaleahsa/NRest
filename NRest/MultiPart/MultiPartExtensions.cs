using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

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
                Encoding encoding = response.Response.ContentEncoding == null
                    ? Encoding.UTF8
                    : Encoding.GetEncoding(response.Response.ContentEncoding);
                StreamingMultiPartParser parser = new StreamingMultiPartParser(responseStream, encoding, boundary);

                MultiPartFileLookup fileLookup = new MultiPartFileLookup();
                NameValueCollection collection = new NameValueCollection();
                parser.SectionFound += (o, e) =>
                {
                    var data = getSectionData(e);
                    if (data == null)
                    {
                        return;
                    }
                    if (String.IsNullOrWhiteSpace(data.FileName))
                    {
                        string value = encoding.GetString(data.Contents);
                        collection.Add(data.Name, value);
                    }
                    else
                    {
                        var file = new MultiPartFile()
                        {
                            Name = data.Name,
                            FileName = data.FileName,
                            ContentType = data.ContentType,
                            Contents = data.Contents
                        };
                        fileLookup.Add(file.Name, file);
                    }
                };
                return new MultiPartResponse() { Files = fileLookup, FormData = collection };
            }
        }

        private static SectionData getSectionData(MultiPartSection section)
        {
            string contentDisposition = section.Headers["Content-Disposition"];
            if (contentDisposition == null)
            {
                return null;
            }
            string[] parts = contentDisposition.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();
            var lookup = parts
                .Select(p => p.Split(new char[] { '=' }, 2))
                .Where(p => p.Length == 2)
                .ToLookup(p => p[0], p => p[1].Trim(' ', '"'), StringComparer.CurrentCultureIgnoreCase);
            SectionData data = new SectionData();
            data.Name = getName(lookup["name"].FirstOrDefault());
            data.FileName = getName(lookup["filename"].FirstOrDefault());
            data.ContentType = section.Headers["Content-Type"];
            data.Contents = copyData(section.Content);
            return data;
        }

        private static string getName(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return value;
        }

        private static byte[] copyData(Stream source)
        {
            MemoryStream destination = new MemoryStream();
            source.CopyTo(destination);
            return destination.ToArray();
        }

        private class SectionData
        {
            public string Name { get; set; }

            public string FileName { get; set; }

            public string ContentType { get; set; }

            public byte[] Contents { get; set; }
        }
    }
}
