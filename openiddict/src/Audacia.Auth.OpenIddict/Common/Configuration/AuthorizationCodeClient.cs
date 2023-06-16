using System;
using System.Collections.Generic;

namespace Audacia.Auth.OpenIddict.Common.Configuration
{
    /// <summary>
    /// Represents an OpenID Connect client that is a UI app, and will use the Authorization Code + PKCE grant type.
    /// </summary>
    public class AuthorizationCodeClient : OpenIdConnectClientBase
    {
        /// <summary>
        /// Gets or sets the base URL that the client will be connecting from.
        /// </summary>
        public Uri BaseUrl { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URLs that the client will be connecting from.
        /// Used for auth-callbacks, slient-renewals, and redirect-uris...
        /// </summary>
        public IReadOnlyCollection<Uri>? RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the URLs that the client will redirect to after a logout is completed.
        /// </summary>
        public IReadOnlyCollection<Uri>? PostLogoutRedirectUris { get; set; }
    }
}
