using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <summary>
    /// Represents a type to handle requests to the /connect/token endpoint.
    /// </summary>
    public interface IGetTokenHandler
    {
        /// <summary>
        /// Handles the given <paramref name="openIddictRequest"/>.
        /// </summary>
        /// <param name="openIddictRequest">The <see cref="OpenIddictRequest"/> object for which to get a token.</param>
        /// <param name="httpRequest">The underlying <see cref="HttpRequest"/>.</param>
        /// <returns>An <see cref="IActionResult"/> object representing the result that should be returned to the client.</returns>
        Task<IActionResult> HandleAsync(OpenIddictRequest openIddictRequest, HttpRequest httpRequest);
    }
}
