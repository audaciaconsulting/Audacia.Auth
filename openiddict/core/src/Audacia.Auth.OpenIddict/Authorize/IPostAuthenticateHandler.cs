using System.Security.Claims;
using System.Threading.Tasks;

namespace Audacia.Auth.OpenIddict.Authorize
{
    /// <summary>
    /// Represents a type that performs some logic after a user has been successfully authenticated.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TId">The type of the user's primary key.</typeparam>
    public interface IPostAuthenticateHandler<TUser, TId>
        where TUser : class
    {
        /// <summary>
        /// Performs the necessary post-authentication logic.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="principal">The authenticated <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        Task HandleAsync(TUser user, ClaimsPrincipal principal);
    }
}
