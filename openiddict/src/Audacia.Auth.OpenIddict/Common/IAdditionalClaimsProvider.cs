using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Represents a type that can obtain claims from a <typeparamref name="TUser"/>.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
public interface IAdditionalClaimsProvider<TUser>
    where TUser : class
{
    /// <summary>
    /// Gets a collection of delegates that, when invoked, get a <see cref="Claim"/>
    /// from the given <typeparamref name="TUser"/>.
    /// </summary>
    IEnumerable<Func<TUser, Claim>> ClaimFactories { get; }
}
