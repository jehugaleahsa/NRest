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

        IMultiPartBodyBuilder WithFile(string name, string fileName, byte[] content, string contentType = null, NameValueCollection headers = null);

        IMultiPartBodyBuilder WithFile(string name, string fileName, Stream fileStream, string contentType = null, NameValueCollection headers = null);

        IMultiPartBodyBuilder WithFile(string name, string filePath, string contentType = null, NameValueCollection headers = null);
    }
}
