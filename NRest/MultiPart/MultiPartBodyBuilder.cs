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
            UrlEncodedBodyBuilder builder = new UrlEncodedBodyBuilder();
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
    }
}
