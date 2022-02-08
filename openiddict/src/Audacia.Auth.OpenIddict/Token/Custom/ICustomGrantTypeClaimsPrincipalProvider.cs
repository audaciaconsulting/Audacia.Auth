namespace Audacia.Auth.OpenIddict.Token.Custom
{
    /// <summary>
    /// Represents a type that can get a <see cref="System.Security.Claims.ClaimsPrincipal"/> for a custom grant type.
    /// </summary>
    public interface ICustomGrantTypeClaimsPrincipalProvider : IClaimsPrincipalProvider
    {
        /// <summary>
        /// Gets the grant type supported by this provider.
        /// </summary>
        string GrantType { get; }
    }
}
