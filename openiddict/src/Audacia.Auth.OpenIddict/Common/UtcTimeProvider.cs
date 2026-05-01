using System;
using System.Diagnostics.CodeAnalysis;

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Default <see cref="IUtcTimeProvider"/> implementaiton.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
internal class UtcTimeProvider : IUtcTimeProvider
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Creates a new <see cref="UtcTimeProvider"/> for .NET 8.0.
    /// </summary>
    /// <param name="timeProvider">The time provider to get the time from.</param>
    public UtcTimeProvider(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public DateTime UtcDateTime => _timeProvider.GetUtcNow().UtcDateTime;
}