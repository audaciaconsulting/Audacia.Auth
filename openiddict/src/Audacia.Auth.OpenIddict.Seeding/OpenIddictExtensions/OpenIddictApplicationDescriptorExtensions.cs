using Audacia.Auth.OpenIddict.Common.Configuration;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Audacia.Auth.OpenIddict.Seeding.OpenIddictExtensions
{
    /// <summary>
    /// Extensions to the 
    /// </summary>
    internal static class OpenIddictApplicationDescriptorExtensions
    {
        /// <summary>
        /// Adds the post logout redirect uris from the given <see cref="AuthorizationCodeClient"/> to the <see cref="OpenIddictApplicationDescriptor"/>.
        /// If no post logout redirect uris are specified in the <see cref="AuthorizationCodeClient"/> then the <see cref="AuthorizationCodeClient.BaseUrl"/> property is used.
        /// </summary>
        /// <param name="applicationClient">The <see cref="OpenIddictApplicationDescriptor"/> object to which to add the post logout redirect uris.</param>
        /// <param name="clientConfig">The <see cref="AuthorizationCodeClient"/> that contains the uris.</param>
        internal static void AddPostLogoutRedirectUris(this OpenIddictApplicationDescriptor applicationClient, AuthorizationCodeClient clientConfig)
        {
            if (clientConfig.PostLogoutRedirectUris == null)
            {
                applicationClient.PostLogoutRedirectUris.Add(clientConfig.BaseUrl);
            }
            else
            {
                foreach (var uri in clientConfig.PostLogoutRedirectUris)
                {
                    applicationClient.PostLogoutRedirectUris.Add(uri);
                }
            }
        }

        /// <summary>
        /// Adds the redirect uris from the given <see cref="AuthorizationCodeClient"/> to the <see cref="OpenIddictApplicationDescriptor"/>.
        /// If no redirect uris are specified in the <see cref="AuthorizationCodeClient"/> then the <see cref="AuthorizationCodeClient.BaseUrl"/> property is used.
        /// </summary>
        /// <param name="applicationClient">The <see cref="OpenIddictApplicationDescriptor"/> object to which to add the redirect uris.</param>
        /// <param name="clientConfig">The <see cref="AuthorizationCodeClient"/> that contains the uris.</param>
        internal static void AddRedirectUris(this OpenIddictApplicationDescriptor applicationClient, AuthorizationCodeClient clientConfig)
        {
            if (clientConfig.RedirectUris == null)
            {
                applicationClient.RedirectUris.Add(clientConfig.BaseUrl);
            }
            else
            {
                foreach (var uri in clientConfig.RedirectUris)
                {
                    applicationClient.RedirectUris.Add(uri);
                }
            }
        }

        /// <summary>
        /// Adds client scopes from the given <see cref="OpenIdConnectClientBase"/> to the <see cref="OpenIddictApplicationDescriptor"/>.
        /// </summary>
        /// <param name="applicationClient">The <see cref="OpenIddictApplicationDescriptor"/> object to which to add the scopes.</param>
        /// <param name="clientConfig">The <see cref="OpenIdConnectClientBase"/> that contains the scopes.</param>
        internal static void AddScopes(this OpenIddictApplicationDescriptor applicationClient, OpenIdConnectClientBase clientConfig)
        {
            if (clientConfig.ClientScopes != null)
            {
                // Define what client scopes this client can access, i.e. booking-api, finance-api
                foreach (var scope in clientConfig.ClientScopes)
                {
                    applicationClient.Permissions.Add(Permissions.Prefixes.Scope + scope);
                }
            }
        }
    }
}
