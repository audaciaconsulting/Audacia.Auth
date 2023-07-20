using Audacia.Auth.OpenIddict.Common.Configuration;

namespace Audacia.Auth.OpenIddict.QuartzCleanup.Configuration;

/// <summary>
/// Represents the <see cref="OpenIdConnectConfig"/> with additional cleanup config.
/// </summary>
public class OpenIdConnectCleanupConfig : OpenIdConnectConfig
{
    /// <summary>
    /// Gets or sets the minimum age of token and authorization to cleanup.
    /// Only tokens and authorizations older than this value will be deleted.
    /// </summary>
    public ConfigurableTimespan? MinimumAgeToCleanup { get; set; }
}
