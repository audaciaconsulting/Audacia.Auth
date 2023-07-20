using System.Threading.Tasks;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Represents a type that can raise auth events.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <param name="authEvent">The event.</param>
    /// <returns>A <see cref="Task"/> representing the operation to raise the event.</returns>
    Task RaiseAsync(AuthEvent authEvent);
}
