using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Audacia.Auth.OpenIddict.UserInfo
{
    /// <summary>
    /// A class that can handle requests to the /connect/userinfo endpoint.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public class DefaultUserInfoHandler<TUser, TKey> : IUserInfoHandler<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAdditionalClaimsProvider<TUser, TKey> _additionalClaimsProvider;

        /// <summary>
        /// Initializes an instance of <see cref="DefaultUserInfoHandler{TUser, TKey}"/>.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="additionalClaimsProvider">An implementation of <see cref="IAdditionalClaimsProvider{TUser, TKey}"/>.</param>
        public DefaultUserInfoHandler(
            UserManager<TUser> userManager,
            IAdditionalClaimsProvider<TUser, TKey> additionalClaimsProvider)
        {
            _userManager = userManager;
            _additionalClaimsProvider = additionalClaimsProvider;
        }

        /// <summary>
        /// Handles the given <paramref name="claimsPrincipal"/> by producing the necessary claims or returning a challenge, as appropriate.
        /// </summary>
        /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> instance from the claims should be extracted.</param>
        /// <returns>An <see cref="IActionResult"/> object representing the result that should be returned to the client.</returns>
        public async Task<IActionResult> HandleAsync(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
            if (user is null)
            {
                return new ChallengeResult(
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The specified access token is bound to an account that no longer exists."
                    }));
            }

            var claims = await GetClaimsAsync(claimsPrincipal, user).ConfigureAwait(false);

            return new OkObjectResult(claims);
        }

        private async Task<Dictionary<string, object>> GetClaimsAsync(ClaimsPrincipal claimsPrincipal, TUser user)
        {
            var claims = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
                [Claims.Subject] = user.Id.ToString()!
            };
            await AddClaimsForRequestedScopesAsync(claimsPrincipal, user, claims).ConfigureAwait(false);
            AddDynamicClaims(user, claims);

            return claims;
        }

        private async Task AddClaimsForRequestedScopesAsync(ClaimsPrincipal claimsPrincipal, TUser user, Dictionary<string, object> claims)
        {
            if (claimsPrincipal.HasScope(Scopes.Email))
            {
                claims[Claims.Email] = user.Email;
                claims[Claims.EmailVerified] = user.EmailConfirmed;
            }

            if (claimsPrincipal.HasScope(Scopes.Phone))
            {
                claims[Claims.PhoneNumber] = user.PhoneNumber;
                claims[Claims.PhoneNumberVerified] = user.PhoneNumberConfirmed;
            }

            if (claimsPrincipal.HasScope(Scopes.Roles))
            {
                claims[Claims.Role] = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            }
        }

        private void AddDynamicClaims(TUser user, Dictionary<string, object> claims)
        {
            // Note: the complete list of standard claims supported by the OpenID Connect specification
            // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

            foreach (var factory in _additionalClaimsProvider.ClaimFactories)
            {
                var dynamicClaim = factory(user);
                claims.TryAdd(dynamicClaim.Type, dynamicClaim.Value);
            }
        }
    }
}
