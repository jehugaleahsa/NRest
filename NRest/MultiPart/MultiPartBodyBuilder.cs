using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NRest.Forms;

namespace NRest.MultiPart
{
    internal class MultiPartBodyBuilder : IMultiPartBodyBuilder, IRequestBodyBuilder
    {
        public const string Boundary = "---------------------------186811537721568";
        private const string newLine = "\r\n";

        private readonly NameValueCollection formData;
        private readonly List<MultiPartFile> fileStreams;
        private readonly Action<IMultiPartBodyBuilder> builder;

        public MultiPartBodyBuilder(Action<IMultiPartBodyBuilder> builder)
        {
            this.formData = new NameValueCollection();
            this.fileStreams = new List<MultiPartFile>();
            this.builder = builder;
        }

        public NameValueCollection FormData 
        {
            get { return formData; }
        }

        public IEnumerable<MultiPartFile> Files
        {
            get { return fileStreams; }
        }

        public IMultiPartBodyBuilder WithFormData(NameValueCollection formData)
        {
            if (formData == null)
            {
                throw new ArgumentNullException("formData");
            }
            formData.Add(formData);
            return this;
        }

        public IMultiPartBodyBuilder WithFormData(object parameters)
        {
            NameValueCollection collection = NameValueCollectionExtensions.CreateNameValueCollection(parameters);
            formData.Add(collection);
            return this;
        }

        public IMultiPartBodyBuilder WithFormData(Action<IUrlEncodedBodyBuilder> formDataBuilder)
        {
            if (formDataBuilder == null)
            {
                throw new ArgumentNullException("formDataBuilder");
            }
            UrlEncodedBodyBuilder builder = new UrlEncodedBodyBuilder(new NameValueCollection());
            formDataBuilder(builder);
            formData.Add(builder.Collection);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string fileName, byte[] content, string contentType = null, NameValueCollection headers = null)
        {
            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                FileName = fileName,
                ContentType = contentType,
                Writer = MultiPartFile.GetStreamWriter(new MemoryStream(content)),
                Headers = headers ?? new NameValueCollection()
            };
            fileStreams.Add(file);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string fileName, Stream fileStream, string contentType = null, NameValueCollection headers = null)
        {

            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                FileName = fileName,
                ContentType = contentType,
                Writer = MultiPartFile.GetStreamWriter(fileStream),
                Headers = headers ?? new NameValueCollection()
            };
            fileStreams.Add(file);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string filePath, string contentType = null, NameValueCollection headers = null)
        {
            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                FileName = Path.GetFileName(filePath),
                ContentType = contentType,
                Writer = MultiPartFile.GetPathWriter(filePath),
                Headers = headers ?? new NameValueCollection()
            };
            fileStreams.Add(file);
            return this;
        }

        void IRequestBodyBuilder.Build(Stream stream, Encoding encoding)
        {
            builder(this);
            StreamWriter writer = new StreamWriter(stream, encoding);
            foreach (string name in FormData.AllKeys)
            {
                writeFormData(writer, FormData, name);
            }
            foreach (MultiPartFile file in Files)
            {
                writeFile(writer, file);
            }
            writeFooter(writer);
        }

        private static void writeFormData(StreamWriter writer, NameValueCollection collection, string name)
        {
            string section = getFormDataSection(collection, name);
            writer.Write(section);
        }

        private static void writeFile(StreamWriter writer, MultiPartFile file)
        {
            string header = getFileSectionHeader(file);
            writer.Write(header);

            file.Writer.Write(writer.BaseStream);
            writer.Write(newLine);
        }

        private static void writeFooter(StreamWriter writer)
        {
            string footer = getFooter();
            writer.Write(footer);
        }

        async Task IRequestBodyBuilder.BuildAsync(Stream stream, Encoding encoding)
        {
            builder(this);
            StreamWriter writer = new StreamWriter(stream, encoding);
            foreach (string name in FormData.AllKeys)
            {
                await writeFormDataAsync(writer, FormData, name);
            }
            foreach (MultiPartFile file in Files)
            {
                await writeFileAsync(writer, file);
            }
            await writeFooterAsync(writer);
        }

        private static async Task writeFormDataAsync(StreamWriter writer, NameValueCollection collection, string name)
        {
            string section = getFormDataSection(collection, name);
            await writer.WriteAsync(section);
        }

        private static async Task writeFileAsync(StreamWriter writer, MultiPartFile file)
        {
            string header = getFileSectionHeader(file);
            await writer.WriteAsync(header);

            await file.Writer.WriteAsync(writer.BaseStream);
            await writer.WriteAsync(newLine);
        }

        private static async Task writeFooterAsync(StreamWriter writer)
        {
            string footer = getFooter();
            await writer.WriteAsync(footer);
        }

        private static string getFormDataSection(NameValueCollection collection, string name)
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Content-Disposition", "form-data");
            headers.Add("Content-Disposition", "name=\"" + escapeQuotes(name) + "\"");

            StringBuilder builder = new StringBuilder();
            builder.Append("--" + Boundary + newLine);

            writeHeaders(builder, headers);

            builder.Append(collection.Get(name));
            builder.Append(newLine);

            return builder.ToString();
        }

        private static string getFileSectionHeader(MultiPartFile file)
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(file.Headers);
            headers.Add("Content-Disposition", "form-data");
            headers.Add("Content-Disposition", "name=\"" + escapeQuotes(file.Name) + "\"");
            headers.Add("Content-Disposition", "filename=\"" + escapeQuotes(file.FileName) + "\"");
            headers.Add("Content-Type", file.ContentType ?? "application/octet-stream");

            StringBuilder builder = new StringBuilder();
            builder.Append("--" + Boundary + newLine);

            writeHeaders(builder, headers);

            return builder.ToString();
        }

        private static string getFooter()
        {
            const string footer = "--" + Boundary + "--" + newLine;
            return footer;
        }

        private static void writeHeaders(StringBuilder builder, WebHeaderCollection headers)
        {
            foreach (string key in headers.AllKeys)
            {
                builder.Append(escape(key));
                builder.Append(": ");
                string[] values = headers.GetValues(key).Select(v => escape(v)).ToArray();
                string joinedValues = String.Join("; ", values);
                builder.Append(joinedValues);
                builder.Append(newLine);
            }
            builder.Append(newLine);
        }

        private static string escape(string value)
        {
            if (value == null)
            {
                return null;
            }
            string result = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
            return result;
        }

        private static string escapeQuotes(string value)
        {
            return value == null ? null : value.Replace("\"", "_");
        }
    }
}
