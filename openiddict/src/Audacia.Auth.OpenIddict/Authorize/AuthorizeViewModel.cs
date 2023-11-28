using System.ComponentModel.DataAnnotations;

namespace Audacia.Auth.OpenIddict.Authorize;

/// <summary>
/// View model representing the data required to display a consent form to the end user.
/// </summary>
public class AuthorizeViewModel
{
    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    [Display(Name = "Application")]
    public string? ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the scope(s) being requested.
    /// </summary>
    [Display(Name = "Scope")]
    public string? Scope { get; set; }
}
