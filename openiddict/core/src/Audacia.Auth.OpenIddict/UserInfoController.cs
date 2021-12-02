using System;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.UserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace Audacia.Auth.OpenIddict
{
    /// <summary>
    /// A controller to handle requests to the /connect/userinfo endpoint.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public class UserInfoController<TUser, TKey> : Controller
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly UserInfoHandler<TUser, TKey> _userInfoHandler;

        /// <summary>
        /// Initializes an instance of <see cref="UserInfoController{TUser, TKey}"/>.
        /// </summary>
        /// <param name="userInfoHandler">A <see cref="UserInfoHandler{TUser, TKey}"/> instance.</param>
        public UserInfoController(UserInfoHandler<TUser, TKey> userInfoHandler)
        {
            _userInfoHandler = userInfoHandler;
        }

        /// <summary>
        /// Carries out the processing to obtain user info.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> object representing the action taken.</returns>
        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        [HttpPost("~/connect/userinfo")]
        [IgnoreAntiforgeryToken]
        [Produces("application/json")]
        public Task<IActionResult> Userinfo()
        {
            return _userInfoHandler.HandleAsync(User);
        }
    }
}
