namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// For specifying if login was interactive.
/// </summary>
public enum LoginType
{
    /// <summary>
    /// Login was not interactive.
    /// </summary>
    NonInteractive = 0,

    /// <summary>
    /// Login was interactive.
    /// </summary>
    Interactive = 100
}
