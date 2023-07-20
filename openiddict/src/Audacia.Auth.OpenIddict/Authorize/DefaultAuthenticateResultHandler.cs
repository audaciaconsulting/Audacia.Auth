using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common;
using Audacia.Auth.OpenIddict.Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Audacia.Auth.OpenIddict.Authorize;

/// <summary>
/// A class that can handle requests to the /connect/authorize endpoint.
/// </summary>
/// <typeparam name="TUser">The type of user.</typeparam>
/// <typeparam name="TId">The type of the user's primary key.</typeparam>
public class DefaultAuthenticateResultHandler<TUser, TId> : IAuthenticateResultHandler<TUser, TId>
    where TUser : class
    where TId : IEquatable<TId>
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly SignInManager<TUser> _signInManager;
    private readonly UserManager<TUser> _userManager;
    private readonly IPostAuthenticateHandler<TUser, TId> _postAuthenticateHandler;

    /// <summary>
    /// Initializes an instance of <see cref="DefaultAuthenticateResultHandler{TUser, TId}"/>.
    /// </summary>
    /// <param name="applicationManager">The <see cref="IOpenIddictApplicationManager"/> instance to use.</param>
    /// <param name="authorizationManager">The <see cref="IOpenIddictAuthorizationManager"/> instance to use.</param>
    /// <param name="scopeManager">The <see cref="IOpenIddictScopeManager"/> instance to use.</param>
    /// <param name="signInManager">The <see cref="SignInManager{TUser}"/> instance to use.</param>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/> instance to use.</param>
    /// <param name="postAuthenticateHandler">The <see cref="IPostAuthenticateHandler{TUser, TKey}"/> instance to use.</param>
    [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "Needs six parameters.")]
    public DefaultAuthenticateResultHandler(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        SignInManager<TUser> signInManager,
        UserManager<TUser> userManager,
        IPostAuthenticateHandler<TUser, TId> postAuthenticateHandler)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _signInManager = signInManager;
        _userManager = userManager;
        _postAuthenticateHandler = postAuthenticateHandler;
    }

    /// <summary>
    /// Handles the given <paramref name="result"/> by prompting the user to login or completing the login process as appropriate.
    /// </summary>
    /// <param name="openIddictRequest">The initial <see cref="OpenIddictRequest"/>.</param>
    /// <param name="httpRequest">The <see cref="HttpRequest"/> object for the current request.</param>
    /// <param name="viewDataDictionary">The <see cref="ViewDataDictionary"/> for the calling controller.</param>
    /// <param name="result">The <see cref="AuthenticateResult"/> object to be handled.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result that should be returned to the client.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">User or client data cannot be found.</exception>
    [SuppressMessage("Maintainability", "ACL1002:Member or local function contains too many statements", Justification = "Could reduce further by returning moving getting the user, application, etc. to helper methods, but I don't think that would actually improve the method.")]
    public virtual async Task<IActionResult> HandleAsync(
        OpenIddictRequest openIddictRequest,
        HttpRequest httpRequest,
        ViewDataDictionary viewDataDictionary,
        AuthenticateResult? result)
    {
        if (openIddictRequest == null) throw new ArgumentNullException(nameof(openIddictRequest));
        if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

        if (result?.Succeeded != true)
        {
            return HandleUnauthenticated(openIddictRequest, httpRequest);
        }

        // If prompt=login was specified by the client application,
        // immediately return the user agent to the login page.
        if (openIddictRequest.HasPrompt(Prompts.Login))
        {
            return HandleLoginPrompt(openIddictRequest, httpRequest);
        }

        // If a max_age parameter was provided, ensure that the cookie is not too old.
        // If it's too old, automatically redirect the user agent to the login page.
        if (CookieTooOld(openIddictRequest, result))
        {
            return HandleCookieTooOld(openIddictRequest, httpRequest);
        }

        // Retrieve the profile of the logged in user.
        var user = await GetUserAsync(result!.Principal!).ConfigureAwait(false);

        // Retrieve the application details from the database.
        var application = await _applicationManager.FindByClientIdAsync(openIddictRequest.ClientId!).ConfigureAwait(false) ??
            throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var applicationId = await _applicationManager.GetIdAsync(application).ConfigureAwait(false) ?? string.Empty;
        var authorizations = await _authorizationManager.FindAsync(
            subject: user?.GetId()?.ToString()!,
            client: applicationId,
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: openIddictRequest.GetScopes()).ToListAsync().ConfigureAwait(false);

        return await ProcessConsentTypesAsync(openIddictRequest, viewDataDictionary, user!, application, applicationId, authorizations).ConfigureAwait(false);
    }

    private async Task<UserWrapper<TUser, TId>> GetUserAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal).ConfigureAwait(false) ??
            throw new InvalidOperationException("The user details cannot be retrieved.");

        return new UserWrapper<TUser, TId>(user);
    }

    [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "All parameters are needed.")]
    private async Task<IActionResult> ProcessConsentTypesAsync(
        OpenIddictRequest openIddictRequest,
        ViewDataDictionary viewDataDictionary,
        UserWrapper<TUser, TId> user,
        object application,
        string applicationId,
        List<object> authorizations)
    {
        switch (await _applicationManager.GetConsentTypeAsync(application).ConfigureAwait(false))
        {
            // If the consent is external (e.g when authorizations are granted by a sysadmin),
            // immediately return an error if no authorization can be found in the database.
            case ConsentTypes.External when !authorizations.Any():
                return ConsentRequiredResult("The logged in user is not allowed to access this client application.");

            // If the consent is implicit or if an authorization was found,
            // return an authorization response without displaying the consent form.
            case ConsentTypes.Implicit:
            case ConsentTypes.External when authorizations.Any():
            case ConsentTypes.Explicit when authorizations.Any() && !openIddictRequest.HasPrompt(Prompts.Consent):
                return await HandleSuccessfulSignInAsync(openIddictRequest, user, applicationId, authorizations).ConfigureAwait(false);

            // At this point, no authorization was found in the database and an error must be returned
            // if the client application specified prompt=none in the authorization request.
            case ConsentTypes.Explicit when openIddictRequest.HasPrompt(Prompts.None):
            case ConsentTypes.Systematic when openIddictRequest.HasPrompt(Prompts.None):
                return ConsentRequiredResult("Interactive user consent is required.");

            // In every other case, render the consent form.
            default:
                return await ConsentFormViewAsync(openIddictRequest, viewDataDictionary, application).ConfigureAwait(false);
        }
    }

    private static IActionResult ConsentRequiredResult(string errorDescription)
    {
        return new ForbidResult(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
            }));
    }

    private async Task<IActionResult> HandleSuccessfulSignInAsync(OpenIddictRequest openIddictRequest, UserWrapper<TUser, TId> user, string applicationId, List<object> authorizations)
    {
        var principal = await _signInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);

        // Note: in this sample, the granted scopes match the requested scope
        // but you may want to allow the user to uncheck specific scopes.
        // For that, simply restrict the list of scopes before calling SetScopes.
        principal.SetScopes(openIddictRequest.GetScopes());
        principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync().ConfigureAwait(false));

        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes.
        var authorization = authorizations.LastOrDefault();
        if (authorization is null)
        {
            authorization = await _authorizationManager.CreateAsync(
                principal: principal,
                subject: user.GetId()!.ToString()!,
                client: applicationId,
                type: AuthorizationTypes.Permanent,
                scopes: principal.GetScopes()).ConfigureAwait(false);
        }

        principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization).ConfigureAwait(false));
        principal.SetDestinations();
        await _postAuthenticateHandler.HandleAsync(user, principal).ConfigureAwait(false);

        return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
    }

    private async Task<IActionResult> ConsentFormViewAsync(OpenIddictRequest openIddictRequest, ViewDataDictionary viewDataDictionary, object application)
    {
        viewDataDictionary.Model = new AuthorizeViewModel
        {
            ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application).ConfigureAwait(false),
            Scope = openIddictRequest.Scope
        };

        return new ViewResult
        {
            ViewName = "Authorize",
            ViewData = viewDataDictionary
        };
    }

    private static IActionResult HandleCookieTooOld(OpenIddictRequest openIddictRequest, HttpRequest httpRequest)
    {
        if (openIddictRequest.HasPrompt(Prompts.None))
        {
            return new ForbidResult(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                }));
        }

        return new ChallengeResult(
            IdentityConstants.ApplicationScheme,
            new AuthenticationProperties
            {
                RedirectUri = httpRequest.CreateRedirectUri()
            });
    }

    private static bool CookieTooOld(OpenIddictRequest openIddictRequest, AuthenticateResult result)
    {
        return 
            openIddictRequest.MaxAge != null &&
            result.Properties?.IssuedUtc != null &&
            DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(openIddictRequest.MaxAge.Value);
    }

    private static IActionResult HandleLoginPrompt(OpenIddictRequest openIddictRequest, HttpRequest httpRequest)
    {
        // To avoid endless login -> authorization redirects, the prompt=login flag
        // is removed from the authorization request payload before redirecting the user.
        var prompt = string.Join(" ", openIddictRequest.GetPrompts().Remove(Prompts.Login));

        var parameters = httpRequest.HasFormContentType ?
            httpRequest.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList() :
            httpRequest.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

        parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

        return new ChallengeResult(
            IdentityConstants.ApplicationScheme,
            new AuthenticationProperties
            {
                RedirectUri = httpRequest.PathBase + httpRequest.Path + QueryString.Create(parameters)
            });
    }

    private static IActionResult HandleUnauthenticated(OpenIddictRequest openIddictRequest, HttpRequest httpRequest)
    {
        // If the client application requested promptless authentication,
        // return an error indicating that the user is not logged in.
        if (openIddictRequest.HasPrompt(Prompts.None))
        {
            return new ForbidResult(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                }));
        }

        return new ChallengeResult(
            IdentityConstants.ApplicationScheme,
            new AuthenticationProperties
            {
                RedirectUri = httpRequest.CreateRedirectUri()
            });
    }
}
