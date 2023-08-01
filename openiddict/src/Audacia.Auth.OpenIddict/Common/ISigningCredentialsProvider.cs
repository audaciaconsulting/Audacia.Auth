using Microsoft.IdentityModel.Tokens;

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Represents a type that can obtain signing credentials.
/// </summary>
public interface ISigningCredentialsProvider
{
    /// <summary>
    /// Gets the credentials that are used to sign tokens.
    /// </summary>
    /// <returns>The <see cref="SigningCredentials"/> object that is used to sign tokens.</returns>
    SigningCredentials GetSigningCredentials();
}
