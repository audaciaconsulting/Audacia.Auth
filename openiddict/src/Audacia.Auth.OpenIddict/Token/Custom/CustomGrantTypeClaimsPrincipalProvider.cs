using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common;
using Audacia.Auth.OpenIddict.Common.Extensions;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token.Custom
{
    /// <summary>
    /// A type that can get the appropriate <see cref="IClaimsPrincipalProvider"/> for a given grant type.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    public class CustomGrantTypeClaimsPrincipalProvider<TUser> : IClaimsPrincipalProvider
        where TUser : class
    {
        private readonly IEnumerable<ICustomGrantTypeValidator<TUser>> _providers;
        private readonly IProfileService<TUser> _profileService;

        /// <summary>
        /// Initializes an instance of <see cref="CustomGrantTypeClaimsPrincipalProvider{TUser}"/>.
        /// </summary>
        /// <param name="providers">The <see cref="ICustomGrantTypeValidator{TUser}"/> instances to use.</param>
        /// <param name="profileService">The service to get custom claims.</param>
        public CustomGrantTypeClaimsPrincipalProvider(IEnumerable<ICustomGrantTypeValidator<TUser>> providers, IProfileService<TUser> profileService)
        {
            _providers = providers;
            _profileService = profileService;
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest)
        {
            if (openIddictRequest == null)
            {
                throw new ArgumentNullException(nameof(openIddictRequest));
            }

            if (openIddictRequest.GrantType is null)
            {
                throw new InvalidGrantException("No grant type is specified.");
            }

            var matchingProvider = _providers.FirstOrDefault(provider => provider.GrantType == openIddictRequest.GrantType);
            if (matchingProvider is null)
            {
                throw new InvalidGrantException($"The grant type '{openIddictRequest.GrantType}' is not supported.");
            }

            var validationResponse = await matchingProvider.ValidateAsync(openIddictRequest).ConfigureAwait(false);
            if (validationResponse?.Principal is null)
            {
                throw new InvalidGrantException($"No principal could be created for the grant type '{openIddictRequest.GrantType}'.");
            }

            await SetAdditionalClaimsPrincipalPropertiesAsync(openIddictRequest, validationResponse).ConfigureAwait(false);

            return validationResponse.Principal;
        }

        private async Task SetAdditionalClaimsPrincipalPropertiesAsync(OpenIddictRequest openIddictRequest, CustomGrantTypeValidationResponse<TUser> validationResponse)
        {
            validationResponse.Principal.SetScopes(openIddictRequest.GetScopes());
            validationResponse.Principal.AddClaims(await _profileService.GetClaimsAsync(validationResponse.User, validationResponse.Principal).ConfigureAwait(false));
        }
    }
}
