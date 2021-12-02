using System;
using System.Collections.Generic;

namespace Audacia.Auth.OpenIddict.Common.Configuration
{
    /// <summary>
    /// A class that describes a client that will be setup in Identity Server.
    /// Can also be used as a container for how to connect to Identity Server.
    /// </summary>
    public class OpenIdConnectClientBase
    {
        /// <summary>
        /// Gets or sets the ClientId of the IdentityServerClient.
        /// </summary>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the scopes that you want to make available to this client.
        /// </summary>
        public IReadOnlyCollection<string> ClientScopes { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets a description of the length of time that an access token is applicable for.
        /// </summary>
        public ConfigurableTimespan? AccessTokenLifetime { get; set; } = null;
    }
}