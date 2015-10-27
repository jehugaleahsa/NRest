using System;
using System.Text;

namespace NRest.Authentication
{
    public static class BasicAuthenticationExtensions
    {
        public static IRequestConfiguration UseBasicAuthentication(this IRequestConfiguration configuration, string userName, string password)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            string combined = String.Format("{0}:{1}", userName, password);
            byte[] data = Encoding.Default.GetBytes(combined);
            string token = Convert.ToBase64String(data);
            string authHeader = string.Format("Basic {0}", token);
            return configuration.WithHeader("Authorization", authHeader);
        }
    }
}
