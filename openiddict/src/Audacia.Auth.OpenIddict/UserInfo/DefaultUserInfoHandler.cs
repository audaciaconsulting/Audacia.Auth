﻿using System;
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

namespace Audacia.Auth.OpenIddict.UserInfo;

/// <summary>
/// A class that can handle requests to the /connect/userinfo endpoint.
/// </summary>
/// <typeparam name="TUser">The type of user.</typeparam>
/// <typeparam name="TId">The type of the user's primary key.</typeparam>
public class DefaultUserInfoHandler<TUser, TId> : IUserInfoHandler<TUser, TId>
    where TUser : class
    where TId : IEquatable<TId>
{
    private readonly UserManager<TUser> _userManager;
    private readonly IProfileService<TUser> _profileService;

    /// <summary>
    /// Initializes an instance of <see cref="DefaultUserInfoHandler{TUser, TId}"/>.
    /// </summary>
    /// <param name="userManager">An instance of <see cref="UserManager{TUser}"/>.</param>
    /// <param name="profileService">An implementation of <see cref="IProfileService{TUser}"/>.</param>
    public DefaultUserInfoHandler(UserManager<TUser> userManager, IProfileService<TUser> profileService)
    {
        _userManager = userManager;
        _profileService = profileService;
    }

    /// <summary>
    /// Handles the given <paramref name="claimsPrincipal"/> by producing the necessary claims or returning a challenge, as appropriate.
    /// </summary>
    /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> instance from the claims should be extracted.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result that should be returned to the client.</returns>
    public virtual async Task<IActionResult> HandleAsync(ClaimsPrincipal claimsPrincipal)
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

        if (!await _profileService.IsActiveAsync(user, claimsPrincipal).ConfigureAwait(false))
        {
            return new ChallengeResult(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that is no longer active."
                }));
        }

        var claims = await GetClaimsAsync(claimsPrincipal, new UserWrapper<TUser, TId>(user)).ConfigureAwait(false);

        return new OkObjectResult(claims);
    }

    private async Task<Dictionary<string, object>> GetClaimsAsync(ClaimsPrincipal claimsPrincipal, UserWrapper<TUser, TId> user)
    {
        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [Claims.Subject] = user.GetId()!.ToString()!
        };
        await AddClaimsForRequestedScopesAsync(claimsPrincipal, user, claims).ConfigureAwait(false);
        await AddDynamicClaimsAsync(user, claimsPrincipal, claims).ConfigureAwait(false);

        return claims;
    }

    private async Task AddClaimsForRequestedScopesAsync(ClaimsPrincipal claimsPrincipal, TUser user, Dictionary<string, object> claims)
    {
        if (claimsPrincipal.HasScope(Scopes.Roles))
        {
            claims[Claims.Role] = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        }
    }

    private async Task AddDynamicClaimsAsync(TUser user, ClaimsPrincipal principal, Dictionary<string, object> claims)
    {
        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        var dynamicClaims = await _profileService.GetClaimsAsync(user, principal).ConfigureAwait(false);
        foreach (var dynamicClaim in dynamicClaims)
        {
            claims.TryAdd(dynamicClaim.Type, dynamicClaim.Value);
        }
    }
}
