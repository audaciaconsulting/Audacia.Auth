using System.Threading.Tasks;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Represents a type that can persist events.
/// </summary>
public interface IEventSink
{
    /// <summary>
    /// Persists the given <paramref name="authEvent"/>.
    /// </summary>
    /// <param name="authEvent">The <see cref="AuthEvent"/> to persist.</param>
    /// <returns>A <see cref="Task"/> representing the operation to persist the event.</returns>
    Task PersistAsync(AuthEvent authEvent);
}
