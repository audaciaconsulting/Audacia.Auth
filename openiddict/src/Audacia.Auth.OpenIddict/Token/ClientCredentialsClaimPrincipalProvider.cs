using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common.Extensions;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <summary>
    /// Implementation of <see cref="IClaimsPrincipalProvider"/> that can get a <see cref="ClaimsPrincipal"/> for the client credentials flow.
    /// </summary>
    public class ClientCredentialsClaimPrincipalProvider : IClaimsPrincipalProvider
    {
        private readonly IOpenIddictApplicationManager _applicationManager;

        private readonly IOpenIddictScopeManager _scopeManager;

        /// <summary>
        /// Initializes an instance of <see cref="ClientCredentialsClaimPrincipalProvider"/>.
        /// </summary>
        /// <param name="applicationManager">An implementation of <see cref="IOpenIddictApplicationManager"/>.</param>
        /// <param name="scopeManager">An implementation of <see cref="IOpenIddictScopeManager"/>.</param>
        public ClientCredentialsClaimPrincipalProvider(
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager)
        {
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest)
        {
            if (openIddictRequest == null) throw new ArgumentNullException(nameof(openIddictRequest));

            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.

            var application = await _applicationManager.FindByClientIdAsync(openIddictRequest.ClientId!)
                .ConfigureAwait(false);
            if (application == null)
            {
                throw new InvalidOperationException("The application details cannot be found in the database.");
            }

            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name,
                Claims.Role);
            await AddAdditionalClientCredentialsClaimsAsync(application, identity).ConfigureAwait(false);

            return await CreatePrincipalForClientCredentialsFlowAsync(openIddictRequest, identity)
                .ConfigureAwait(false);
        }

        private async Task AddAdditionalClientCredentialsClaimsAsync(object application, ClaimsIdentity identity)
        {
            var clientId = await _applicationManager.GetClientIdAsync(application).ConfigureAwait(false);
            var displayName = await _applicationManager.GetDisplayNameAsync(application).ConfigureAwait(false);

            // Use the client_id as the subject identifier.
            identity.SetClaim(Claims.Subject, clientId);
            identity.SetClaim(Claims.Name, displayName);

            identity.SetDestinations(claim =>
            {
                return claim.Type switch
                {
                    Claims.Subject => new[]
                    {
                        Destinations.AccessToken,
                        Destinations.IdentityToken
                    },

                    // Allow the "name" claim to be stored in both the access and identity tokens
                    // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                    Claims.Name when displayName != null
                        => new[]
                        {
                            Destinations.AccessToken,
                            Destinations.IdentityToken
                        }
                };
            });
        }

        private async Task<ClaimsPrincipal> CreatePrincipalForClientCredentialsFlowAsync(
            OpenIddictRequest openIddictRequest, ClaimsIdentity identity)
        {
            // Note: In the original OAuth 2.0 specification, the client credentials grant
            // doesn't return an identity token, which is an OpenID Connect concept.
            //
            // As a non-standardized extension, OpenIddict allows returning an id_token
            // to convey information about the client application when the "openid" scope
            // is granted (i.e specified when calling principal.SetScopes()). When the "openid"
            // scope is not explicitly set, no identity token is returned to the client application.

            // Set the list of scopes granted to the client application in access_token.
            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(openIddictRequest.GetScopes());
            principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes())
                .ToListAsync()
                .ConfigureAwait(false));

            return principal;
        }
    }
}
