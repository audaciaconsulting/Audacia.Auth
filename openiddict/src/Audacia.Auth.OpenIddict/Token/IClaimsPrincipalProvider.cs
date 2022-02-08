using System.Security.Claims;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <summary>
    /// Represents a type that can obtain a <see cref="ClaimsPrincipal"/> from an <see cref="OpenIddictRequest"/>.
    /// </summary>
    public interface IClaimsPrincipalProvider
    {
        /// <summary>
        /// Asynchronously gets a <see cref="ClaimsPrincipal"/> object from the given <paramref name="openIddictRequest"/>.
        /// If a principal cannot be created then an <see cref="InvalidGrantException"/> should be thrown.
        /// </summary>
        /// <param name="openIddictRequest">The <see cref="OpenIddictRequest"/> from which to obtain the <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> that, when completed, contains a <see cref="ClaimsPrincipal"/> object.</returns>
        Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest);
    }
}
