using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Event for failed token issuance.
/// </summary>
/// <seealso cref="AuthEvent" />
public class TokenIssuedFailureEvent : AuthEvent
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// The client identifier.
    /// </value>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI.
    /// </summary>
    /// <value>
    /// The redirect URI.
    /// </value>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? RedirectUri { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    /// <summary>
    /// Gets or sets the endpoint.
    /// </summary>
    /// <value>
    /// The endpoint.
    /// </value>
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the scopes.
    /// </summary>
    /// <value>
    /// The scopes.
    /// </value>
    public string? Scopes { get; set; }

    /// <summary>
    /// Gets or sets the grant type.
    /// </summary>
    /// <value>
    /// The grant type.
    /// </value>
    public string? GrantType { get; set; }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>
    /// The error.
    /// </value>
    public string Error { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenIssuedFailureEvent"/> class.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="error">The error.</param>
    public TokenIssuedFailureEvent(OpenIddictRequest request, string error)
        : base(EventCategories.Token, "Token Issued Failure", EventTypes.Failure, EventIds.TokenIssuedFailure)
    {
        if (request != null)
        {
            ClientId = request.ClientId;
            RedirectUri = request.RedirectUri;
            Scopes = request.Scope;
            GrantType = request.GrantType;
        }

        Endpoint = EndpointNames.Authorize;
        Error = error;
    }
}
