using System.Diagnostics.CodeAnalysis;
using Audacia.Auth.OpenIddict.Common.Extensions;

namespace Audacia.Auth.OpenIddict.Common.Events
{
    /// <summary>
    /// Event for failed user authentication.
    /// </summary>
    /// <seealso cref="AuthEvent" />
    public class UserLoginFailureEvent : AuthEvent
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the subject Id.
        /// </summary>
        /// <value>
        /// The subject Id.
        /// </value>
        public string? SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public string? ClientId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginFailureEvent" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="error">The error.</param>
        /// <param name="interactive">Specifies if login was interactive.</param>
        /// <param name="clientId">The client id.</param>
#pragma warning disable AV1564 // Parameter in public or internal member is of type bool or bool?
        public UserLoginFailureEvent(string username, string error, bool interactive = true, string? clientId = null)
#pragma warning restore AV1564 // Parameter in public or internal member is of type bool or bool?
            : base(EventCategories.Authentication, "User Login Failure", EventTypes.Failure, EventIds.UserLoginFailure,
                  error)
        {
            Username = username.Obfuscate();
            ClientId = clientId;

            if (interactive)
            {
                Endpoint = "UI";
            }
            else
            {
                Endpoint = EndpointNames.Token;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginFailureEvent" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="subjectId">The subject Id.</param>
        /// <param name="error">The error.</param>
        /// <param name="interactive">Specifies if login was interactive.</param>
        /// <param name="clientId">The client id.</param>
#pragma warning disable AV1564 // Parameter in public or internal member is of type bool or bool?
        [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "Needs all parameters.")]
        public UserLoginFailureEvent(string username, string? subjectId, string error, bool interactive = true, string? clientId = null)
#pragma warning restore AV1564 // Parameter in public or internal member is of type bool or bool?
            : base(EventCategories.Authentication, "User Login Failure", EventTypes.Failure, EventIds.UserLoginFailure,
                  error)
        {
            Username = username.Obfuscate();
            SubjectId = subjectId;
            ClientId = clientId;

            if (interactive)
            {
                Endpoint = "UI";
            }
            else
            {
                Endpoint = EndpointNames.Token;
            }
        }
    }
}
