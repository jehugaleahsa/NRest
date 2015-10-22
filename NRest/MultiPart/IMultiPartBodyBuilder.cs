using System;
using System.Collections.Specialized;
using System.IO;
using NRest.Forms;

namespace NRest.MultiPart
{
    public interface IMultiPartBodyBuilder
    {
        IMultiPartBodyBuilder WithFormData(NameValueCollection formData);

        IMultiPartBodyBuilder WithFormData(Action<IUrlEncodedBodyBuilder> formDataBuilder);

        IMultiPartBodyBuilder WithFile(string name, string path, byte[] content, string contentType = null);

        IMultiPartBodyBuilder WithFile(string name, string path, Stream fileStream, string contentType = null);

        IMultiPartBodyBuilder WithFile(string name, string path, string filePath, string contentType = null);
    }
}
