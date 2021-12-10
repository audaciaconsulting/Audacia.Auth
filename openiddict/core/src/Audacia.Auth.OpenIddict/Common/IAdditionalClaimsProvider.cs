using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Represents a type that can obtain claims from a <typeparamref name="TUser"/>.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public interface IAdditionalClaimsProvider<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets a collection of delegates that, when invoked, get a <see cref="Claim"/>
        /// from the given <typeparamref name="TUser"/>.
        /// </summary>
        IEnumerable<Func<TUser, Claim>> ClaimFactories { get; }
    }
}
