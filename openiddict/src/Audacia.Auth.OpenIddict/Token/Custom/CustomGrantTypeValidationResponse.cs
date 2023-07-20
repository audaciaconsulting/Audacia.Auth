using System.Security.Claims;

namespace Audacia.Auth.OpenIddict.Token.Custom;

/// <summary>
/// Class containing the data returned by a custom grant type validator.
/// </summary>
/// <typeparam name="TUser">The type of user.</typeparam>
public class CustomGrantTypeValidationResponse<TUser> where TUser : class
{
    /// <summary>
    /// Gets the authenticated <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public ClaimsPrincipal Principal { get; }

    /// <summary>
    /// Gets the current <typeparamref name="TUser"/>.
    /// </summary>
    public TUser User { get; }

    /// <summary>
    /// Initializes an instance of <see cref="CustomGrantTypeValidationResponse{TUser}"/>.
    /// </summary>
    /// <param name="principal">The authenticated <see cref="ClaimsPrincipal"/>.</param>
    /// <param name="user">The current <typeparamref name="TUser"/>.</param>
    public CustomGrantTypeValidationResponse(ClaimsPrincipal principal, TUser user)
    {
        Principal = principal;
        User = user;
    }
}
