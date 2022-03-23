using System;
using System.Collections.Generic;

namespace Audacia.Auth.OpenIddict.Common.Configuration
{
    /// <summary>
    /// Represents an OpenID Connect client that uses a custom grant type.
    /// </summary>
    public class CustomGrantTypeClient : OpenIdConnectClientBase
    {
        /// <summary>
        /// Gets or sets the grant type used by the client.
        /// </summary>
        public string GrantType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the URLs that the client will be connecting from.
        /// </summary>
        public IReadOnlyCollection<Uri>? ClientUris { get; set; }
    }
}
