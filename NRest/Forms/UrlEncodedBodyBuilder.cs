using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NRest.Forms
{
    internal class UrlEncodedBodyBuilder : IUrlEncodedBodyBuilder, IRequestBodyBuilder
    {
        private readonly NameValueCollection collection;
        private readonly Action<IUrlEncodedBodyBuilder> collectionBuilder;

        public UrlEncodedBodyBuilder(Action<IUrlEncodedBodyBuilder> collectionBuilder)
        {
            this.collection = new NameValueCollection();
            this.collectionBuilder = collectionBuilder;
        }

        public UrlEncodedBodyBuilder(NameValueCollection collection)
        {
            this.collection = collection;
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

        void IRequestBodyBuilder.Build(Stream stream, Encoding encoding)
        {
            string serialized = getQueryString();
            StreamWriter writer = new StreamWriter(stream, encoding);
            writer.Write(serialized);
        }

        async Task IRequestBodyBuilder.BuildAsync(Stream stream, Encoding encoding)
        {
            string serialized = getQueryString();
            StreamWriter writer = new StreamWriter(stream, encoding);
            await writer.WriteAsync(serialized);
        }

        private string getQueryString()
        {
            if (collectionBuilder != null)
            {
                collectionBuilder(this);
            }
            string serialized = Collection.ToQueryString();
            return serialized;
        }
    }
}
