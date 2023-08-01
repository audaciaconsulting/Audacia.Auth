using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Audacia.Auth.OpenIddict.UserInfo;

/// <summary>
/// Represents a type that can handle requests to the /connect/userinfo endpoint.
/// </summary>
/// <typeparam name="TUser">The type of user.</typeparam>
/// <typeparam name="TId">The type of the user's primary key.</typeparam>
public interface IUserInfoHandler<TUser, TId>
    where TUser : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Handles the given <paramref name="claimsPrincipal"/>.
    /// </summary>
    /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> instance from the claims should be extracted.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result that should be returned to the client.</returns>
    Task<IActionResult> HandleAsync(ClaimsPrincipal claimsPrincipal);
}
