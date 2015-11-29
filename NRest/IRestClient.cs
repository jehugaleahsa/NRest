using System;

namespace NRest
{
    public interface IRestClient
    {
        void Configure(Action<IRequestConfiguration> configurator);

        IRequestConfiguration Connect(string uriString, object uriParameters = null);

        IRequestConfiguration CreateRequest(string method, string uriString, object uriParameters = null);

        IRequestConfiguration Delete(string uriString, object uriParameters = null);

        IRequestConfiguration Get(string uriString, object uriParameters = null);

        IRequestConfiguration Head(string uriString, object uriParameters = null);

        IRequestConfiguration Options(string uriString, object uriParameters = null);

        IRequestConfiguration Patch(string uriString, object uriParameters = null);

        IRequestConfiguration Post(string uriString, object uriParameters = null);

        IRequestConfiguration Put(string uriString, object uriParameters = null);

        IRequestConfiguration Trace(string uriString, object uriParameters = null);
    }
}
