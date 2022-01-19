namespace Audacia.Auth.OpenIddict.Common.Configuration
{
    /// <summary>
    /// Represents an OpenID Connect client that is a test automation client, and will use the Resource Owner Password Credential grant type.
    /// </summary>
    public class ResourceOwnerPasswordClient : OpenIdConnectClientBase
    {
        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;
    }
}
