using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Audacia.Auth.OpenIddict.Common.Events;
using Audacia.Auth.OpenIddict.Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <summary>
    /// A class that can handle requests to the /connect/token endpoint.
    /// </summary>
    public class DefaultGetTokenHandler : IGetTokenHandler
    {
        private readonly OpenIdConnectConfig _openIdConnectConfig;
        private readonly IClaimsPrincipalProviderFactory _claimsPrincipalProviderFactory;
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes an instance of <see cref="DefaultGetTokenHandler"/>.
        /// </summary>
        /// <param name="openIdConnectConfig">The configuration.</param>
        /// <param name="claimsPrincipalProviderFactory">An instance of <see cref="IClaimsPrincipalProviderFactory"/>.</param>
        /// <param name="eventService">An instance of <see cref="IEventService"/> that can be used to write events.</param>
        public DefaultGetTokenHandler(
            OpenIdConnectConfig openIdConnectConfig,
            IClaimsPrincipalProviderFactory claimsPrincipalProviderFactory,
            IEventService eventService)
        {
            _openIdConnectConfig = openIdConnectConfig;
            _claimsPrincipalProviderFactory = claimsPrincipalProviderFactory;
            _eventService = eventService;
        }

        /// <summary>
        /// Handles the given <paramref name="openIddictRequest"/> by issuing a token or returning an error result as appropriate.
        /// </summary>
        /// <param name="openIddictRequest">The <see cref="OpenIddictRequest"/> object for which to get a token.</param>
        /// <param name="httpRequest">The underlying <see cref="HttpRequest"/>.</param>
        /// <returns>An <see cref="IActionResult"/> object representing the result that should be returned to the client.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="openIddictRequest"/> is <see langword="null"/>.</exception>
        public virtual async Task<IActionResult> HandleAsync(OpenIddictRequest openIddictRequest, HttpRequest httpRequest)
        {
            if (openIddictRequest == null) throw new ArgumentNullException(nameof(openIddictRequest));

            ClaimsPrincipal principal;
            try
            {
                principal = await GetPrincipalAsync(openIddictRequest).ConfigureAwait(false);
            }
            catch (InvalidGrantException invalidGrantException)
            {
                await _eventService.RaiseAsync(new TokenIssuedFailureEvent(openIddictRequest, invalidGrantException.Message)).ConfigureAwait(false);

                return InvalidGrantResult(invalidGrantException);
            }

            principal.SetDestinations();
            AdjustAccessTokenLifetime(openIddictRequest, principal);
            await _eventService.RaiseAsync(new TokenIssuedSuccessEvent(EndpointNames.Token, principal, openIddictRequest)).ConfigureAwait(false);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
        }

        private Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest)
        {
            var claimsPrincipalProvider = _claimsPrincipalProviderFactory.CreateProvider(openIddictRequest);
            
            return claimsPrincipalProvider.GetPrincipalAsync(openIddictRequest);
        }

        private static IActionResult InvalidGrantResult(InvalidGrantException exception)
        {
            return new ForbidResult(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = exception.Message
                }));
        }

        private void AdjustAccessTokenLifetime(OpenIddictRequest openIddictRequest, ClaimsPrincipal principal)
        {
            // Adjust access token lifetimes if configured to be different in the appsettings
            if (!string.IsNullOrEmpty(openIddictRequest.ClientId) &&
                _openIdConnectConfig.TryFindClient(openIddictRequest.ClientId, out var clientConfig) &&
                clientConfig?.AccessTokenLifetime != null)
            {
                var accessTokenLifetime = clientConfig.AccessTokenLifetime.GetLifetime(nameof(clientConfig.AccessTokenLifetime));
                principal.SetAccessTokenLifetime(accessTokenLifetime);
            }
        }
    }
}
