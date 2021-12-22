using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Audacia.Auth.OpenIddict.Authorize
{
    /// <summary>
    /// The default implementation of <see cref="IPostAuthenticateHandler{TUser, TId}"/> that performs no logic.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TId">The type of the user's primary key.</typeparam>
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
    internal class DefaultPostAuthenticateHandler<TUser, TId> : IPostAuthenticateHandler<TUser, TId>
        where TUser : class
    {
        /// <inheritdoc />
        public Task HandleAsync(TUser user, ClaimsPrincipal principal)
        {
            return Task.CompletedTask;
        }
    }
}
