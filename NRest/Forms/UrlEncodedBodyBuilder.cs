using System;
using System.Collections.Specialized;
using System.IO;
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

        void IRequestBodyBuilder.Build(Stream stream)
        {
            string serialized = getQueryString();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(serialized);
            writer.Flush();
        }

        async Task IRequestBodyBuilder.BuildAsync(Stream stream)
        {
            string serialized = getQueryString();
            StreamWriter writer = new StreamWriter(stream);
            await writer.WriteAsync(serialized);
            await writer.FlushAsync();
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
