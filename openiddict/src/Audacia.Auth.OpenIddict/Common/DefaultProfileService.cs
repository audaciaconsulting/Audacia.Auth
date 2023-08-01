using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Default implementation of <see cref="IProfileService{TUser}"/>.
/// </summary>
/// <typeparam name="TUser">The type of user.</typeparam>
public class DefaultProfileService<TUser> : IProfileService<TUser>
    where TUser : class
{
    private readonly IAdditionalClaimsProvider<TUser> _additionalClaimsProvider;

    /// <summary>
    /// Initializes an instance of <see cref="DefaultProfileService{TUser}"/>.
    /// </summary>
    /// <param name="additionalClaimsProvider">The claims provider from which to get claims.</param>
    public DefaultProfileService(IAdditionalClaimsProvider<TUser> additionalClaimsProvider)
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
