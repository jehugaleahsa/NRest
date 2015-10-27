using System;
using NRest.Forms;

namespace NRest.Authentication
{
    public static class OAuth2AuthenticationExtensions
    {
        public static IRequestConfiguration UseOAuth2QueryStringAuthentication(this IRequestConfiguration configuration, string accessToken)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration.WithQueryParameter("oauth_token", accessToken);
        }

        public static IUrlEncodedBodyBuilder UseOAuth2Authentication(this IUrlEncodedBodyBuilder builder, string accessToken)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            return builder.WithParameter("oauth_token", accessToken);
        }

        public static IRequestConfiguration UseOAuth2HeaderAuthentication(this IRequestConfiguration configuration, string accessToken)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration.WithHeader("Authorization", accessToken);
        }

        public static IRequestConfiguration UseOAuth2HeaderAuthentication(this IRequestConfiguration configuration, string accessToken, string tokenType)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            return configuration.WithHeader("Authorization", tokenType + " " + accessToken);
        }
    }
}
