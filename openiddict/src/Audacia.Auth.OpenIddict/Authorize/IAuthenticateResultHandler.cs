using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Authorize;

/// <summary>
/// Represents a type that can handle requests to the /connect/authorize endpoint.
/// </summary>
/// <typeparam name="TUser">The type of user.</typeparam>
/// <typeparam name="TId">The type of the user's primary key.</typeparam>
public interface IAuthenticateResultHandler<TUser, TId>
    where TUser : class
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Handles the given <paramref name="result"/>.
    /// </summary>
    /// <param name="openIddictRequest">The initial <see cref="OpenIddictRequest"/>.</param>
    /// <param name="httpRequest">The <see cref="HttpRequest"/> object for the current request.</param>
    /// <param name="viewDataDictionary">The <see cref="ViewDataDictionary"/> for the calling controller.</param>
    /// <param name="result">The <see cref="AuthenticateResult"/> object to be handled.</param>
    /// <returns>An <see cref="IActionResult"/> object representing the result that should be returned to the client.</returns>
    Task<IActionResult> HandleAsync(
        OpenIddictRequest openIddictRequest,
        HttpRequest httpRequest,
        ViewDataDictionary viewDataDictionary,
        AuthenticateResult? result);
}
