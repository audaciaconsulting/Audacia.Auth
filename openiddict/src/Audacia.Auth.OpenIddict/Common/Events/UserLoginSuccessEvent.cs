using System.Diagnostics.CodeAnalysis;
using Audacia.Auth.OpenIddict.Common.Extensions;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Event for successful user authentication.
/// </summary>
/// <seealso cref="AuthEvent" />
public class UserLoginSuccessEvent : AuthEvent
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>
    /// The username.
    /// </value>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the provider.
    /// </summary>
    /// <value>
    /// The provider.
    /// </value>
    public string? Provider { get; set; }

    /// <summary>
    /// Gets or sets the provider user identifier.
    /// </summary>
    /// <value>
    /// The provider user identifier.
    /// </value>
    public string? ProviderUserId { get; set; }

    /// <summary>
    /// Gets or sets the subject identifier.
    /// </summary>
    /// <value>
    /// The subject identifier.
    /// </value>
    public string? SubjectId { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the endpoint.
    /// </summary>
    /// <value>
    /// The endpoint.
    /// </value>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the client id.
    /// </summary>
    /// <value>
    /// The client id.
    /// </value>
    public string? ClientId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLoginSuccessEvent"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="providerUserId">The provider user identifier.</param>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="interactive">if set to <see langword="true"/> [interactive].</param>
    [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "All parameters required.")]
#pragma warning disable AV1564 // Parameter in public or internal member is of type bool or bool?
    public UserLoginSuccessEvent(string provider, string providerUserId, string subjectId, string name, bool interactive = true)
#pragma warning restore AV1564 // Parameter in public or internal member is of type bool or bool?
        : this()
    {
        Provider = provider;
        ProviderUserId = providerUserId;
        SubjectId = subjectId;
        DisplayName = name.Obfuscate();
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
    /// Initializes a new instance of the <see cref="UserLoginSuccessEvent"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="providerUserId">The provider user identifier.</param>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="interactive">if set to <see langword="true"/> [interactive].</param>
    /// <param name="clientId">The client id.</param>
    [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "All parameters required.")]
#pragma warning disable AV1564 // Parameter in public or internal member is of type bool or bool?
    public UserLoginSuccessEvent(string provider, string providerUserId, string subjectId, string name, bool interactive, string? clientId)
#pragma warning restore AV1564 // Parameter in public or internal member is of type bool or bool?
        : this()
    {
        Provider = provider;
        ProviderUserId = providerUserId;
        SubjectId = subjectId;
        DisplayName = name.Obfuscate();
        if (interactive)
        {
            Endpoint = "UI";
        }
        else
        {
            Endpoint = EndpointNames.Token;
        }

        ClientId = clientId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLoginSuccessEvent"/> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="interactive">if set to <see langword="true"/> [interactive].</param>
#pragma warning disable AV1564 // Parameter in public or internal member is of type bool or bool?
    public UserLoginSuccessEvent(string username, string subjectId, string name, bool interactive = true)
#pragma warning restore AV1564 // Parameter in public or internal member is of type bool or bool?
        : this()
    {
        Username = username.Obfuscate();
        SubjectId = subjectId;
        DisplayName = name.Obfuscate();

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
    /// Initializes a new instance of the <see cref="UserLoginSuccessEvent"/> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="interactive">if set to <see langword="true"/> [interactive].</param>
    /// <param name="clientId">The client id.</param>
    [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "All parameters required.")]
#pragma warning disable AV1564 // Parameter in public or internal member is of type bool or bool?
    public UserLoginSuccessEvent(string username, string subjectId, string name, bool interactive, string? clientId)
#pragma warning restore AV1564 // Parameter in public or internal member is of type bool or bool?
        : this()
    {
        Username = username.Obfuscate();
        SubjectId = subjectId;
        DisplayName = name.Obfuscate();
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
    /// Initializes a new instance of the <see cref="UserLoginSuccessEvent"/> class.
    /// </summary>
    private UserLoginSuccessEvent()
        : base(EventCategories.Authentication, "User Login Success", EventTypes.Success, EventIds.UserLoginSuccess)
    {
    }
}
