using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Default implementation of <see cref="IAdditionalClaimsProvider{TUser}"/> that doesn't add any additional claims.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
internal class DefaultAdditionalClaimsProvider<TUser> : IAdditionalClaimsProvider<TUser>
    where TUser : class
{
    /// <inheritdoc />
    public IEnumerable<Func<TUser, Claim>> ClaimFactories { get; } = Enumerable.Empty<Func<TUser, Claim>>();
}
