using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common;
using Audacia.Auth.OpenIddict.Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <summary>
    /// Implementation of <see cref="IClaimsPrincipalProvider"/> that can get a <see cref="ClaimsPrincipal"/> for code exchange,
    /// e.g. exchanging an authorization code for a token.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public class CodeExchangeClaimsPrincipalProvider<TUser, TKey> : IClaimsPrincipalProvider
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IProfileService<TUser, TKey> _profileService;
        private readonly HttpContext _httpContext;

        /// <summary>
        /// Initializes an instance of <see cref="CodeExchangeClaimsPrincipalProvider{TUser, TKey}"/>.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="signInManager">An instance of <see cref="SignInManager{TUser}"/>.</param>
        /// <param name="profileService">An instance of <see cref="IProfileService{TUser, TKey}"/>.</param>
        /// <param name="httpContextAccessor">An instance of <see cref="IHttpContextAccessor"/>.</param>
        public CodeExchangeClaimsPrincipalProvider(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IProfileService<TUser, TKey> profileService,
            IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            _userManager = userManager;
            _signInManager = signInManager;
            _profileService = profileService;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest)
        {
            // Retrieve the claims principal stored in the authorization code/device code/refresh token.
            var principal = (await _httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme).ConfigureAwait(false)).Principal!;

            // Retrieve the user profile corresponding to the authorization code/refresh token.
            // Note: if you want to automatically invalidate the authorization code/refresh token
            // when the user password/roles change, use the following line instead:
            // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
            var user = await _userManager.GetUserAsync(principal).ConfigureAwait(false);
            if (user is null)
            {
                throw new InvalidGrantException("The token is no longer valid.");
            }

            // Ensure the user is still allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user).ConfigureAwait(false))
            {
                throw new InvalidGrantException("The user is no longer allowed to sign in.");
            }

            principal.AddClaims(await _profileService.GetClaimsAsync(user, principal).ConfigureAwait(false));

            return principal;
        }
    }
}
