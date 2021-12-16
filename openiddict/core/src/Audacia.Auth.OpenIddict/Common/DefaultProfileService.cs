using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Default implementation of <see cref="IProfileService{TUser, TKey}"/>.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public class DefaultProfileService<TUser, TKey> : IProfileService<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IAdditionalClaimsProvider<TUser, TKey> _additionalClaimsProvider;

        /// <summary>
        /// Initializes an instance of <see cref="DefaultProfileService{TUser, TKey}"/>.
        /// </summary>
        /// <param name="additionalClaimsProvider">The claims provider from which to get claims.</param>
        public DefaultProfileService(IAdditionalClaimsProvider<TUser, TKey> additionalClaimsProvider)
        {
            _additionalClaimsProvider = additionalClaimsProvider;
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<Claim>> GetClaimsAsync(TUser user, ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));

            var claims = new List<Claim>();
            foreach (var factory in _additionalClaimsProvider.ClaimFactories)
            {
                claims.Add(factory(user));
            }

            return Task.FromResult<IEnumerable<Claim>>(claims);
        }

        /// <inheritdoc />
        public virtual Task<bool> IsActiveAsync(TUser user, ClaimsPrincipal claimsPrincipal)
        {
            return Task.FromResult(true);
        }
    }
}
