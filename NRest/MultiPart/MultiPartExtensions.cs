using System;
using System.Collections.Specialized;
using System.IO;

namespace NRest.MultiPart
{
    public static class MultiPartExtensions
    {
        private const string boundary = "---------------------------186811537721568";
        private const string newLine = "\r\n";

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
                .ConfigureRequest(r => r.ContentType = "multipart/form-data; boundary=" + boundary)
                .WithBodyBuilder(stream =>
                {
                    MultiPartBodyBuilder builder = new MultiPartBodyBuilder();
                    multiPartBuilder(builder);
                    StreamWriter writer = new StreamWriter(stream);
                    foreach (string name in builder.FormData.AllKeys)
                    {
                        writeFormData(writer, builder.FormData, name);
                    }
                    foreach (MultiPartFile file in builder.Files)
                    {
                        writeFile(writer, file);
                    }
                    writeFooter(writer);
                    writer.Flush();
                });
        }

        private static void writeFormData(StreamWriter writer, NameValueCollection collection, string name)
        {
            writer.Write("--");
            writer.Write(boundary);
            writer.Write(newLine);
            writer.Write("Content-Disposition: form-data; name=\"");
            writer.Write(name);
            writer.Write("\"");
            writer.Write(newLine);
            writer.Write(newLine);
            writer.Write(collection.Get(name));
            writer.Write(newLine);
        }

        private static void writeFile(StreamWriter writer, MultiPartFile file)
        {
            writer.Write("--");
            writer.Write(boundary);
            writer.Write(newLine);
            writer.Write("Content-Disposition: form-data; name=\"");
            writer.Write(file.Name);
            writer.Write("\"");
            writer.Write("; filename=\"");
            writer.Write(file.FileName);
            writer.Write("\"");
            writer.Write(newLine);
            writer.Write("Content-Type: ");
            writer.Write(file.ContentType ?? "application/octet-stream");
            writer.Write(newLine);
            writer.Write(newLine);
            writer.Flush();  // flush before directly accessing stream
            file.Writer(writer.BaseStream);
            writer.Write(newLine);
        }

        private static void writeFooter(StreamWriter writer)
        {
            writer.Write("--");
            writer.Write(boundary);
            writer.Write("--");
            writer.Write(newLine);
        }
    }
}
