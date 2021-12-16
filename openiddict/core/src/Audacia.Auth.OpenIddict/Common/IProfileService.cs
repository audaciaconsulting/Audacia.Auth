using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Represents a type that can add additional claims for a user.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public interface IProfileService<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Adds additional claims to the given <paramref name="claimsPrincipal"/>.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <param name="claimsPrincipal">The current <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> which, when complete, contains the claims.</returns>
        Task<IEnumerable<Claim>> GetClaimsAsync(TUser user, ClaimsPrincipal claimsPrincipal);

        /// <summary>
        /// Checks whether or not the given <paramref name="user"/> is active.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <param name="claimsPrincipal">The current <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing the result <see langword="true"/> if the user is active, or <see langword="false"/> if the user is not active.</returns>
        Task<bool> IsActiveAsync(TUser user, ClaimsPrincipal claimsPrincipal);
    }
}
