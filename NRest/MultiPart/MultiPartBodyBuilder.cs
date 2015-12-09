using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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

        public IMultiPartBodyBuilder WithFile(string name, string fileName, byte[] content, string contentType = null)
        {
            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                FileName = fileName,
                ContentType = contentType,
                Writer = MultiPartFile.GetStreamWriter(new MemoryStream(content))
            };
            fileStreams.Add(file);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string fileName, Stream fileStream, string contentType = null)
        {

            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                FileName = fileName,
                ContentType = contentType,
                Writer = MultiPartFile.GetStreamWriter(fileStream)
            };
            fileStreams.Add(file);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string path, string contentType = null)
        {
            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                FileName = Path.GetFileName(path),
                ContentType = contentType,
                Writer = MultiPartFile.GetPathWriter(path)
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

        private static string getFormDataSection(NameValueCollection collection, string name)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("--" + Boundary);
            builder.Append(newLine);
            builder.Append("Content-Disposition: form-data; name=\"" + name + "\"");
            builder.Append(newLine + newLine);
            builder.Append(collection.Get(name));
            builder.Append(newLine);
            return builder.ToString();
        }

        private static void writeFile(StreamWriter writer, MultiPartFile file)
        {
            string header = getFileSectionHeader(file);
            writer.Write(header);

            file.Writer.Write(writer.BaseStream);
            writer.Write(newLine);
        }

        private static string getFileSectionHeader(MultiPartFile file)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("--" + Boundary);
            builder.Append(newLine);
            builder.Append("Content-Disposition: form-data; name=\"" + file.Name + "\"; filename=\"" + file.FileName + "\"");
            builder.Append(newLine);
            builder.Append("Content-Type: " + (file.ContentType ?? "application/octet-stream"));
            builder.Append(newLine + newLine);
            return builder.ToString();
        }

        private static void writeFooter(StreamWriter writer)
        {
            string footer = getFooter();
            writer.Write(footer);
        }

        private static string getFooter()
        {
            const string footer = "--" + Boundary + "--" + newLine;
            return footer;
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
    }
}
