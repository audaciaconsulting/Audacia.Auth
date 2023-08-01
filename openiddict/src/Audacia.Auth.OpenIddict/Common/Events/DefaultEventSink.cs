using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Default implementation of <see cref="IEventSink"/> that logs the event.
/// </summary>
public class DefaultEventSink : IEventSink
{
    private readonly ILogger<DefaultEventSink> _logger;
    private readonly IEventSerializer _eventSerializer;

    /// <summary>
    /// Initializes an instance of <see cref="DefaultEventSink"/>.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="eventSerializer">The <see cref="IEventSerializer"/> object that can serialize events for logging.</param>
    public DefaultEventSink(ILogger<DefaultEventSink> logger, IEventSerializer eventSerializer)
    {
        _logger = logger;
        _eventSerializer = eventSerializer;
    }

    /// <inheritdoc />
    public virtual Task PersistAsync(AuthEvent authEvent)
    {
        var output = _eventSerializer.Serialize(authEvent);
        _logger.LogInformation(output);

        return Task.CompletedTask;
    }
}
