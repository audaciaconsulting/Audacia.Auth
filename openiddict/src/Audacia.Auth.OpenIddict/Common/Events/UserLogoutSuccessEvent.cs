using Audacia.Auth.OpenIddict.Common.Extensions;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Event for successful user logout.
/// </summary>
/// <seealso cref="AuthEvent" />
public class UserLogoutSuccessEvent : AuthEvent
{
    /// <summary>
    /// Gets or sets the subject identifier.
    /// </summary>
    /// <value>
    /// The subject identifier.
    /// </value>
    public string SubjectId { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string DisplayName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLogoutSuccessEvent"/> class.
    /// </summary>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="name">The name.</param>
    public UserLogoutSuccessEvent(string subjectId, string name)
        : base(EventCategories.Authentication, "User Logout Success", EventTypes.Success, EventIds.UserLogoutSuccess)
    {
        SubjectId = subjectId;
        DisplayName = name.Obfuscate();
    }
}
