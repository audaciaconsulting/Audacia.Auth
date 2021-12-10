using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Default implementation of <see cref="IAdditionalClaimsProvider{TUser, TKey}"/> that gets basic claims from the given user.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public class DefaultAdditionalClaimsProvider<TUser, TKey> : IAdditionalClaimsProvider<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <inheritdoc />
        public IEnumerable<Func<TUser, Claim>> ClaimFactories
        {
            get
            {
                yield return user => new Claim(OpenIddictConstants.Claims.Email, user.Email);
                foreach (var factory in CustomClaimFactories)
                {
                    yield return factory;
                }
            }
        }

        /// <summary>
        /// Gets the claim factories to add to the default factories provided by <see cref="ClaimFactories"/>.
        /// </summary>
        protected virtual IEnumerable<Func<TUser, Claim>> CustomClaimFactories => Enumerable.Empty<Func<TUser, Claim>>();
    }
}
