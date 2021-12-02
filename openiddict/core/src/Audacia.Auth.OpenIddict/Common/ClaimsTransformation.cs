﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.UserInfo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Appends claims generated by the <see cref="IAdditionalClaimsProvider{TUser, TKey}"/> to the existing <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public class ClaimsTransformation<TUser, TKey> : IClaimsTransformation
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IAdditionalClaimsProvider<TUser, TKey> _additionalClaimsProvider;
        private readonly UserManager<TUser> _userManager;
        
        /// <summary>
        /// Only needs to be run once per request.
        /// </summary>
        private bool _isTransformed = false;

        /// <summary>
        /// Initializes an instance of <see cref="ClaimsTransformation{TUser, TKey}"/>.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="additionalClaimsProvider">An implementation of <see cref="IAdditionalClaimsProvider{TUser, TKey}"/>.</param>
        public ClaimsTransformation(
            UserManager<TUser> userManager,
            IAdditionalClaimsProvider<TUser, TKey> additionalClaimsProvider)
        {
            _userManager = userManager;
            _additionalClaimsProvider = additionalClaimsProvider;
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            if (_isTransformed || !(principal.Identity is ClaimsIdentity identity))
            {
                return principal;
            }

            var user = await FindUserAsync(principal).ConfigureAwait(false);
            AppendDynamicClaims(identity, user);

            return principal;
        }

        private async Task<TUser?> FindUserAsync(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(Claims.Subject);

            // Ignore claims transformation for the ApiClient
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _))
            {
                return default;
            }

            return await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
        }

        private void AppendDynamicClaims(ClaimsIdentity identity, TUser? user)
        {
            if (user != null)
            {
                foreach (var factory in _additionalClaimsProvider.ClaimFactories)
                {
                    var claim = factory(user);
                    if (!identity.HasClaim(claim.Type, claim.Value))
                    {
                        identity.AddClaim(claim);
                    }
                }

                _isTransformed = true;
            }
        }
    }
}
