using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Audacia.Auth.OpenIddict.Common.Extensions;

/// <summary>
/// Extensions to the <see cref="ClaimsPrincipal"/> type.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Sets the destinations for the claims in the given <paramref name="principal"/>.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> for whose claims the destinations should be set.</param>
    internal static void SetDestinations(this ClaimsPrincipal principal)
    {
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "ACL1002:Member or local function contains too many statements", Justification = "Easier to understand, not worth breaking up.")]
    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case Claims.Name:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Profile))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Email))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Roles))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }

    /// <summary>
    /// Converts the claims in the given <paramref name="principal"/> to a dictionary.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> from which to get the claims.</param>
    /// <returns>An <see cref="IDictionary{TKey, TValue}"/> representing the claims of the given <paramref name="principal"/> as key-value pairs.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("member-design", "AV1130:Return interfaces to unchangeable collections.", Justification = "Collection is limited to dictionaries, as defined in the method name")]
    internal static IDictionary<string, string> ToClaimsDictionary(this ClaimsPrincipal principal) =>
        principal.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

    /// <summary>
    /// Adds the given <paramref name="claims"/> to the given <paramref name="principal"/>, provided its identity is a <see cref="ClaimsIdentity"/>.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> to which to add the claims.</param>
    /// <param name="claims">The <see cref="Claim"/> objects to add to the <paramref name="principal"/>.</param>
    internal static void AddClaims(this ClaimsPrincipal principal, IEnumerable<Claim> claims)
    {
        if (principal.Identity is ClaimsIdentity identity)
        {
            identity.AddClaims(claims);
        }
    }

    /// <summary>
    /// Gets the subject Id claim from the given <paramref name="claimsPrincipal"/>.
    /// </summary>
    /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> in which to find the subject Id.</param>
    /// <returns>The subject Id if one is found, otherwise returns <see langword="null"/>.</returns>
    internal static string? GetSubjectId(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.FindFirst(Claims.Subject);

        return claim?.Value;
    }
}
