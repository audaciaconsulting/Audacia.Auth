using System;
using System.Security.Claims;
using Audacia.Auth.OpenIddict.Common.Extensions;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Common.Events
{
    /// <summary>
    /// Event for successful token issuance.
    /// </summary>
    /// <seealso cref="AuthEvent" />
    public class TokenIssuedSuccessEvent : AuthEvent
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
        public string? Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        public string? SubjectId { get; set; }

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
        /// Initializes a new instance of the <see cref="TokenIssuedSuccessEvent"/> class.
        /// </summary>
        /// <param name="endpointName">The name of the endpoint called.</param>
        /// <param name="claimsPrincipal">The authenticate <see cref="ClaimsPrincipal"/>.</param>
        /// <param name="request">The request.</param>
        public TokenIssuedSuccessEvent(string endpointName, ClaimsPrincipal claimsPrincipal, OpenIddictRequest request)
            : base(EventCategories.Token, "Token Issued Success", EventTypes.Success, EventIds.TokenIssuedSuccess)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
            if (request == null) throw new ArgumentNullException(nameof(request));

            ClientId = request.ClientId;
            Endpoint = endpointName;
            SubjectId = claimsPrincipal.GetSubjectId();
            GrantType = request.GrantType;
            Scopes = request.Scope;
        }
    }
}
