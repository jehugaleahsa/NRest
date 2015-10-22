using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using NRest.Forms;

namespace NRest.MultiPart
{
    internal class MultiPartBodyBuilder : IMultiPartBodyBuilder
    {
        private NameValueCollection formData;
        private readonly List<MultiPartFile> fileStreams;

        public MultiPartBodyBuilder()
        {
            this.formData = new NameValueCollection();
            this.fileStreams = new List<MultiPartFile>();
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
            formData.Add(formData);
            return this;
        }

        public IMultiPartBodyBuilder WithFormData(Action<IUrlEncodedBodyBuilder> formDataBuilder)
        {
            UrlEncodedBodyBuilder builder = new UrlEncodedBodyBuilder();
            formDataBuilder(builder);
            formData.Add(builder.Collection);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string path, byte[] content, string contentType = null)
        {
            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                Path = path,
                ContentType = contentType,
                Writer = MultiPartFile.GetStreamWriter(new MemoryStream(content))
            };
            fileStreams.Add(file);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string path, Stream fileStream, string contentType = null)
        {
            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                Path = path,
                ContentType = contentType,
                Writer = MultiPartFile.GetStreamWriter(fileStream)
            };
            fileStreams.Add(file);
            return this;
        }

        public IMultiPartBodyBuilder WithFile(string name, string path, string filePath, string contentType = null)
        {
            MultiPartFile file = new MultiPartFile()
            {
                Name = name,
                Path = path,
                ContentType = contentType,
                Writer = MultiPartFile.GetPathWriter(filePath)
            };
            fileStreams.Add(file);
            return this;
        }
    }
}
