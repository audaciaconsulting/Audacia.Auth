using Microsoft.IdentityModel.Tokens;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Represents a type that can obtain encryption credentials.
    /// </summary>
    public interface IEncryptionCredentialsProvider
    {
        /// <summary>
        /// Gets the credentials that are used to encrypt tokens.
        /// </summary>
        /// <returns>The <see cref="EncryptingCredentials"/> object that is used to encrypt tokens.</returns>
        EncryptingCredentials GetEncryptionCredentials();
    }
}
