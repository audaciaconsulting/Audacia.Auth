using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Default implementation of <see cref="IEventService"/>.
/// </summary>
public class DefaultEventService : IEventService
{
    private readonly IEventSink _eventSink;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ISystemClock _clock;

    /// <summary>
    /// Initializes an instance of <see cref="DefaultEventService"/>.
    /// </summary>
    /// <param name="eventSink">An <see cref="IEventSink"/> to persist events.</param>
    /// <param name="contextAccessor">An object from which the <see cref="HttpContext"/> can be obtained.</param>
    /// <param name="clock">The system clock.</param>
    public DefaultEventService(IEventSink eventSink, IHttpContextAccessor contextAccessor, ISystemClock clock)
    {
        _eventSink = eventSink;
        _contextAccessor = contextAccessor;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task RaiseAsync(AuthEvent authEvent)
    {
        if (authEvent == null) throw new ArgumentNullException(nameof(authEvent));

        await PrepareEventAsync(authEvent).ConfigureAwait(false);
        await _eventSink.PersistAsync(authEvent).ConfigureAwait(false);
    }

    /// <summary>
    /// Prepares the given <paramref name="authEvent"/> for persistence by setting various properties.
    /// </summary>
    /// <param name="authEvent">The <see cref="AuthEvent"/> to prepare.</param>
    /// <returns>A <see cref="Task"/> representing the operation.</returns>
    /// <exception cref="ArgumentNullException">The given <paramref name="authEvent"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The current <see cref="HttpContext"/> is <see langword="null"/>.</exception>
    protected virtual Task PrepareEventAsync(AuthEvent authEvent)
    {
        if (authEvent == null) throw new ArgumentNullException(nameof(authEvent));

        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new InvalidOperationException("HttpContext cannot be null.");
        }

        authEvent.ActivityId = httpContext.TraceIdentifier;
        authEvent.TimeStamp = _clock.UtcNow.UtcDateTime;
        authEvent.ProcessId = Environment.ProcessId;
        SetIpAddressProperties(httpContext, authEvent);            

        return authEvent.PrepareAsync();
    }

    private static void SetIpAddressProperties(HttpContext httpContext, AuthEvent authEvent)
    {
        if (httpContext.Connection.LocalIpAddress != null)
        {
            authEvent.LocalIpAddress = httpContext.Connection.LocalIpAddress.ToString() + ":" + httpContext.Connection.LocalPort;
        }
        else
        {
            authEvent.LocalIpAddress = "unknown";
        }

        if (httpContext.Connection.RemoteIpAddress != null)
        {
            authEvent.RemoteIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
        }
        else
        {
            authEvent.RemoteIpAddress = "unknown";
        }
    }
}
