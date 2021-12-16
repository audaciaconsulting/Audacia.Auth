using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <summary>
    /// Implementation of <see cref="IClaimsPrincipalProvider"/> that can get a <see cref="ClaimsPrincipal"/> for the resource owner password credential flow.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    public class PasswordClaimsPrincipalProvider<TUser, TKey> : IClaimsPrincipalProvider
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly ILogger<PasswordClaimsPrincipalProvider<TUser, TKey>> _logger;

        /// <summary>
        /// Initializes an instance of <see cref="PasswordClaimsPrincipalProvider{TUser, TKey}"/>.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="signInManager">An instance of <see cref="SignInManager{TUser}"/>.</param>
        /// <param name="scopeManager">An instance of <see cref="IOpenIddictScopeManager"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger"/>.</param>
        public PasswordClaimsPrincipalProvider(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IOpenIddictScopeManager scopeManager,
            ILogger<PasswordClaimsPrincipalProvider<TUser, TKey>> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _scopeManager = scopeManager;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest)
        {
            if (openIddictRequest == null) throw new ArgumentNullException(nameof(openIddictRequest));

            var user = await _userManager.FindByNameAsync(openIddictRequest.Username!).ConfigureAwait(false);
            if (user is null)
            {
                throw new InvalidGrantException("The username/password couple is invalid.");
            }

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = await _signInManager.CheckPasswordSignInAsync(user, openIddictRequest.Password, lockoutOnFailure: true).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                HandleFailedSignIn(result, user);
            }

            return await CreatePrincipalForPasswordFlowAsync(openIddictRequest, user).ConfigureAwait(false);
        }

        private void HandleFailedSignIn(SignInResult result, TUser user)
        {
            if (result.IsLockedOut)
            {
                _logger.LogInformation("User {UserId} is locked out.", user.Id);
            }
            else if (result.IsNotAllowed)
            {
                _logger.LogInformation("User {UserId} is not allowed to sign in.", user.Id);
            }
            else
            {
                _logger.LogInformation("Invalid credentials for user {UserId}.", user.Id);
            }

            throw new InvalidGrantException("The username/password couple is invalid.");
        }

        private async Task<ClaimsPrincipal> CreatePrincipalForPasswordFlowAsync(OpenIddictRequest openIddictRequest, TUser user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);

            principal.SetScopes(openIddictRequest.GetScopes());
            principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync().ConfigureAwait(false));

            return principal;
        }
    }
}
