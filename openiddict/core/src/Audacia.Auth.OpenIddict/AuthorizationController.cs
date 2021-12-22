using System;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Authorize;
using Audacia.Auth.OpenIddict.Token;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Audacia.Auth.OpenIddict
{
    /// <summary>
    /// A controller to handle requests to the /connect/authorize and /connect/token endpoints.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TId">The type of the user's primary key.</typeparam>
    public class AuthorizationController<TUser, TId> : Controller
        where TUser : class
    {
        private readonly IAuthenticateResultHandler<TUser, TId> _authenticateResultHandler;
        private readonly IGetTokenHandler _getTokenHandler;

        /// <summary>
        /// Initializes an instance of <see cref="AuthorizationController{TUser, TId}"/>.
        /// </summary>
        /// <param name="authenticateResultHandler">The <see cref="DefaultAuthenticateResultHandler{TUser, TId}"/> instance.</param>
        /// <param name="getTokenHandler">The <see cref="IGetTokenHandler"/> instance.</param>
        public AuthorizationController(
            IAuthenticateResultHandler<TUser, TId> authenticateResultHandler,
            IGetTokenHandler getTokenHandler)
        {
            _authenticateResultHandler = authenticateResultHandler;
            _getTokenHandler = getTokenHandler;
        }

        /// <summary>
        /// Carries out the authorization process.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> object representing the action taken.</returns>
        /// <exception cref="InvalidOperationException">If the OpenID Connect request is not found.</exception>
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var openIddictRequest = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the user principal stored in the authentication cookie.
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme).ConfigureAwait(false);

            return await _authenticateResultHandler.HandleAsync(openIddictRequest, Request, ViewData, result).ConfigureAwait(false);
        }

        /// <summary>
        /// Carries out the process for a client to obtain a token.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> object representing the action taken.</returns>
        /// <exception cref="InvalidOperationException">If the OpenID Connect request is not found.</exception>
        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public Task<IActionResult> Exchange()
        {
            var openIddictRequest = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            return _getTokenHandler.HandleAsync(openIddictRequest, Request);
        }
    }
}
