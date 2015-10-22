using System;

namespace NRest.Forms
{
    public interface IUrlEncodedBodyBuilder
    {
        IUrlEncodedBodyBuilder WithParameter(string name, string value);

        IUrlEncodedBodyBuilder WithParameter(string name, int? value);    }
}
