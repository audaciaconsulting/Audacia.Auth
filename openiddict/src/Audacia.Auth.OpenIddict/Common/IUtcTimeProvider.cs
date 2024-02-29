using System;

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Provides the current UTC time.
/// </summary>
public interface IUtcTimeProvider
{
    /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    DateTime UtcDateTime { get; }
}