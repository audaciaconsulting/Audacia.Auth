using System;
using System.Diagnostics.CodeAnalysis;
#if NET6_0
using Microsoft.AspNetCore.Authentication;
#endif

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Default <see cref="IUtcTimeProvider"/> implementaiton.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
internal class UtcTimeProvider : IUtcTimeProvider
{
#if NET6_0
    private readonly ISystemClock _clock;

    /// <summary>
    /// Creates a new <see cref="UtcTimeProvider"/> for .NET 6.0.
    /// </summary>
    /// <param name="clock">The clock to get the time from.</param>
    public UtcTimeProvider(ISystemClock clock)
    {
        _clock = clock;
    }

    /// <inheritdoc />
    public DateTime UtcDateTime => _clock.UtcNow.UtcDateTime;

#elif NET8_0
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
#endif
}