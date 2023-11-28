namespace Audacia.Auth.OpenIddict.Common.Configuration;

/// <summary>
/// Represents an OpenID Connect client that is an API, and will use the Client Credentials grant type.
/// </summary>
public class ClientCredentialsClient : OpenIdConnectClientBase
{
    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
