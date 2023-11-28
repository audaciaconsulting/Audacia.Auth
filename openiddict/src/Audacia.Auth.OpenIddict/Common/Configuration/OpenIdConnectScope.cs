using System.Collections.Generic;

namespace Audacia.Auth.OpenIddict.Common.Configuration;

/// <summary>
/// Represents the configuration necessary to register scopes for the Open ID Connect server.
/// </summary>
public class OpenIdConnectScope
{
    /// <summary>
    /// Gets or sets the name of the scope.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the api clients that have permission to use this scope.
    /// </summary>
    public IReadOnlyCollection<string>? Resources { get; set; }
}
