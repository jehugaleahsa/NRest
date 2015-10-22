using System.Collections.Specialized;

namespace NRest.Forms
{
    internal class UrlEncodedBodyBuilder : IUrlEncodedBodyBuilder
    {
        private readonly NameValueCollection collection;

        public UrlEncodedBodyBuilder()
        {
            this.collection = new NameValueCollection();
        }

        public NameValueCollection Collection 
        { 
            get { return collection; } 
        }

        public IUrlEncodedBodyBuilder WithParameter(string name, string value)
        {
            collection.Add(name, value);
            return this;
        }

        public IUrlEncodedBodyBuilder WithParameter(string name, int? value)
        {
            collection.Add(name, value.ToString());
            return this;
        }
    }
}
