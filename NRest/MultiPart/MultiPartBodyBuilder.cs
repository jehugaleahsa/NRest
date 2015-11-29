using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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

        void IRequestBodyBuilder.Build(Stream stream)
        {
            builder(this);
            StreamWriter writer = new StreamWriter(stream);
            foreach (string name in FormData.AllKeys)
            {
                writeFormData(writer, FormData, name);
            }
            foreach (MultiPartFile file in Files)
            {
                writeFile(writer, file);
            }
            writeFooter(writer);
            writer.Flush();
        }

        private static void writeFormData(StreamWriter writer, NameValueCollection collection, string name)
        {
            writer.Write("--" + Boundary);
            writer.Write(newLine);
            writer.Write("Content-Disposition: form-data; name=\"" + name + "\"");
            writer.Write(newLine + newLine);
            writer.Write(collection.Get(name));
            writer.Write(newLine);
        }

        private static void writeFile(StreamWriter writer, MultiPartFile file)
        {
            writer.Write("--" + Boundary);
            writer.Write(newLine);
            writer.Write("Content-Disposition: form-data; name=\"" + file.Name + "\"; filename=\"" + file.FileName + "\"");
            writer.Write(newLine);
            writer.Write("Content-Type: " + (file.ContentType ?? "application/octet-stream"));
            writer.Write(newLine + newLine);
            writer.Flush();  // flush before directly accessing stream
            file.Writer.Write(writer.BaseStream);
            writer.Write(newLine);
        }

        private static void writeFooter(StreamWriter writer)
        {
            writer.Write("--" + Boundary + "--");
            writer.Write(newLine);
        }

        async Task IRequestBodyBuilder.BuildAsync(Stream stream)
        {
            builder(this);
            StreamWriter writer = new StreamWriter(stream);
            foreach (string name in FormData.AllKeys)
            {
                await writeFormDataAsync(writer, FormData, name);
            }
            foreach (MultiPartFile file in Files)
            {
                await writeFileAsync(writer, file);
            }
            await writeFooterAsync(writer);
            await writer.FlushAsync();
        }

        private static async Task writeFormDataAsync(StreamWriter writer, NameValueCollection collection, string name)
        {
            await writer.WriteAsync("--" + Boundary);
            await writer.WriteAsync(newLine);
            await writer.WriteAsync("Content-Disposition: form-data; name=\"" + name + "\"");
            await writer.WriteAsync(newLine + newLine);
            await writer.WriteAsync(collection.Get(name));
            await writer.WriteAsync(newLine);
        }

        private static async Task writeFileAsync(StreamWriter writer, MultiPartFile file)
        {
            await writer.WriteAsync("--" + Boundary);
            await writer.WriteAsync(newLine);
            await writer.WriteAsync("Content-Disposition: form-data; name=\"" + file.Name + "\"; filename=\"" + file.FileName + "\"");
            await writer.WriteAsync(newLine);
            await writer.WriteAsync("Content-Type: " + (file.ContentType ?? "application/octet-stream"));
            await writer.WriteAsync(newLine + newLine);
            await writer.FlushAsync();  // flush before directly accessing stream
            await file.Writer.WriteAsync(writer.BaseStream);
            await writer.WriteAsync(newLine);
        }

        private static async Task writeFooterAsync(StreamWriter writer)
        {
            await writer.WriteAsync("--" + Boundary + "--");
            await writer.WriteAsync(newLine);
        }
    }
}
