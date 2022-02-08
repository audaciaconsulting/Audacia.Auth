using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token.Custom
{
    /// <summary>
    /// A type that can get the appropriate <see cref="IClaimsPrincipalProvider"/> for a given grant type.
    /// </summary>
    public class CustomGrantTypeClaimsPrincipalProvider : IClaimsPrincipalProvider
    {
        private readonly IEnumerable<ICustomGrantTypeClaimsPrincipalProvider> _providers;

        /// <summary>
        /// Initializes an instance of <see cref="CustomGrantTypeClaimsPrincipalProvider"/>.
        /// </summary>
        /// <param name="providers">The <see cref="ICustomGrantTypeClaimsPrincipalProvider"/> instances to use.</param>
        public CustomGrantTypeClaimsPrincipalProvider(IEnumerable<ICustomGrantTypeClaimsPrincipalProvider> providers)
        {
            _providers = providers;
        }

        /// <inheritdoc />
        public Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest)
        {
            if (openIddictRequest == null)
            {
                throw new ArgumentNullException(nameof(openIddictRequest));
            }

            var grantType = openIddictRequest.GrantType;
            if (grantType is null)
            {
                throw new InvalidGrantException("No grant type is specified.");
            }

            var matchingProvider = _providers.FirstOrDefault(provider => provider.GrantType == grantType);
            if (matchingProvider is null)
            {
                throw new InvalidGrantException($"The grant type '{grantType}' is not supported.");
            }

            return matchingProvider.GetPrincipalAsync(openIddictRequest);
        }
    }
}
