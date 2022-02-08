using System;
using System.Collections.Generic;
using System.Linq;
using Audacia.Auth.OpenIddict.Common.Extensions;

namespace Audacia.Auth.OpenIddict.Common.Configuration
{
    /// <summary>
    /// Represents the OpenIdConnectConfig section of the app settings.
    /// Contains the configuration necessary to use the Audacia.Auth.OpenIddict package.
    /// </summary>
    public class OpenIdConnectConfig
    {
        private ICollection<OpenIdConnectClientBase>? _clients;

        /// <summary>
        /// Gets or sets the thumbprint of the certificate to use for token encryption.
        /// This should NOT be the same as the <see cref="SigningCertificateThumbprint"/>.
        /// </summary>
        public string? EncryptionCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the thumbprint of the certificate to use for token signing.
        /// This should NOT be the same as the <see cref="EncryptionCertificateThumbprint"/>.
        /// </summary>
        public string? SigningCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the base url for the OpenID Connect server.
        /// </summary>
        public Uri Url { get; set; } = null!;

        /// <summary>
        /// Gets or sets the configuration to be used by API clients with the Client Credentials flow.
        /// </summary>
        public IReadOnlyCollection<ClientCredentialsClient>? ClientCredentialsClients { get; set; }

        /// <summary>
        /// Gets or sets the configuration to be used by UI clients with the Authorization Code + PKCE flow.
        /// </summary>
        public IReadOnlyCollection<AuthorizationCodeClient>? AuthorizationCodeClients { get; set; }

        /// <summary>
        /// Gets or sets the configuration to be used by Test Automation clients with the Resource Owner Password Credential (ROPC) flow.
        /// </summary>
        public IReadOnlyCollection<ResourceOwnerPasswordClient>? ResourceOwnerPasswordClients { get; set; }

        /// <summary>
        /// Gets or sets the scopes to be registered for the Open ID Connect server.
        /// </summary>
        public IReadOnlyCollection<OpenIdConnectScope>? Scopes { get; set; }

        /// <summary>
        /// Gets or sets the custom grant types supported by the Open ID Connect server.
        /// </summary>
        public IReadOnlyCollection<string>? CustomGrantTypes { get; set; }

        /// <summary>
        /// Gets all configured clients.
        /// </summary>
        public IEnumerable<OpenIdConnectClientBase> AllClients
        {
            get
            {
                return _clients ??= ClientCredentialsClients.EmptyIfNull().Cast<OpenIdConnectClientBase>()
                                .Union(AuthorizationCodeClients.EmptyIfNull().Cast<OpenIdConnectClientBase>())
                                .Union(ResourceOwnerPasswordClients.EmptyIfNull().Cast<OpenIdConnectClientBase>())
                                .ToList();
            }
        }

        /// <summary>
        /// Looks for a client with the given <paramref name="clientId"/>.
        /// </summary>
        /// <param name="clientId">The ID of the client to look for.</param>
        /// <param name="client">An instance of <see cref="OpenIdConnectClientBase"/> representing the client, if found; otherwise <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the client is found, otherwise <see langword="false"/>.</returns>
        public bool TryFindClient(string clientId, out OpenIdConnectClientBase? client)
        {
            client = AllClients.FirstOrDefault(client => client.ClientId == clientId);
            
            return client != null;
        }
    }
}