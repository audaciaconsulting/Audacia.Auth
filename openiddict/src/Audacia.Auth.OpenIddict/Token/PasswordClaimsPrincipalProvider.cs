using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common;
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
    /// <typeparam name="TId">The type of the user's primary key.</typeparam>
    public class PasswordClaimsPrincipalProvider<TUser, TId> : IClaimsPrincipalProvider
        where TUser : class
        where TId : IEquatable<TId>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly IProfileService<TUser> _profileService;
        private readonly ILogger<PasswordClaimsPrincipalProvider<TUser, TId>> _logger;

        /// <summary>
        /// Initializes an instance of <see cref="PasswordClaimsPrincipalProvider{TUser, TId}"/>.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="signInManager">An instance of <see cref="SignInManager{TUser}"/>.</param>
        /// <param name="scopeManager">An instance of <see cref="IOpenIddictScopeManager"/>.</param>
        /// <param name="profileService">An instance of <see cref="IProfileService{TUser}"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger"/>.</param>
        [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "Needs five parameters.")]
        public PasswordClaimsPrincipalProvider(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IOpenIddictScopeManager scopeManager,
            IProfileService<TUser> profileService,
            ILogger<PasswordClaimsPrincipalProvider<TUser, TId>> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _scopeManager = scopeManager;
            _profileService = profileService;
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
            var userProxy = new UserWrapper<TUser, TId>(user);
            var result = await _signInManager.CheckPasswordSignInAsync(user, openIddictRequest.Password, lockoutOnFailure: true).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                HandleFailedSignIn(result, userProxy);
            }

            return await CreatePrincipalForPasswordFlowAsync(openIddictRequest, userProxy).ConfigureAwait(false);
        }

        private void HandleFailedSignIn(SignInResult result, UserWrapper<TUser, TId> user)
        {
            if (result.IsLockedOut)
            {
                _logger.LogInformation("User {UserId} is locked out.", user.GetId());
            }
            else if (result.IsNotAllowed)
            {
                _logger.LogInformation("User {UserId} is not allowed to sign in.", user.GetId());
            }
            else
            {
                _logger.LogInformation("Invalid credentials for user {UserId}.", user.GetId());
            }

            throw new InvalidGrantException("The username/password couple is invalid.");
        }

        private async Task<ClaimsPrincipal> CreatePrincipalForPasswordFlowAsync(OpenIddictRequest openIddictRequest, UserWrapper<TUser, TId> user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);
            if (!await _profileService.IsActiveAsync(user, principal).ConfigureAwait(false))
            {
                _logger.LogInformation("User {UserId} is not active.", user.GetId());
                throw new InvalidGrantException("The user is not active.");
            }

            principal.SetScopes(openIddictRequest.GetScopes());
            principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync().ConfigureAwait(false));
            principal.AddClaims(await _profileService.GetClaimsAsync(user, principal).ConfigureAwait(false));

            return principal;
        }
    }
}
