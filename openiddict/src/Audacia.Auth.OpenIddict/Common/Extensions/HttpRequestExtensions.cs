using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Audacia.Auth.OpenIddict.Common.Extensions;

/// <summary>
/// Extensions to the <see cref="HttpRequest"/> type.
/// </summary>
internal static class HttpRequestExtensions
{
    /// <summary>
    /// Creates a redirect uri from the given <paramref name="httpRequest"/>.
    /// </summary>
    /// <param name="httpRequest">The current <see cref="HttpRequest"/> instance.</param>
    /// <returns>A <see cref="string"/> representing the post authentication redirect uri.</returns>
    internal static string CreateRedirectUri(this HttpRequest httpRequest) =>
        httpRequest.PathBase + httpRequest.Path + httpRequest.CreateQueryString();

    private static QueryString CreateQueryString(this HttpRequest httpRequest) =>
        QueryString.Create(httpRequest.HasFormContentType ? httpRequest.Form.ToList() : httpRequest.Query.ToList());
}
