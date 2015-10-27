using System;
using System.Net;

namespace NRest.Authentication
{
    public static class NtlmAuthenticationExtensions
    {
        public static IRequestConfiguration UseNtlmAuthentication(this IRequestConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration.UsingDefaultCredentials(true);
        }

        public static IRequestConfiguration UseNtlmAuthentication(this IRequestConfiguration configuration, string userName, string password)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            NetworkCredential credential = new NetworkCredential(userName, password);
            return configuration.WithCredentials(credential);
        }
    }
}
