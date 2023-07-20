using System;
using System.Threading.Tasks;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Base class that represents an event that can be raised.
/// </summary>
public abstract class AuthEvent
{
    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    /// <value>
    /// The category.
    /// </value>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    /// <value>
    /// The type of the event.
    /// </value>
    public EventTypes EventType { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the event message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the per-request activity identifier.
    /// </summary>
    /// <value>
    /// The activity identifier.
    /// </value>
    public string? ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the time stamp when the event was raised.
    /// </summary>
    /// <value>
    /// The time stamp.
    /// </value>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the server process identifier.
    /// </summary>
    /// <value>
    /// The process identifier.
    /// </value>
    public int ProcessId { get; set; }

    /// <summary>
    /// Gets or sets the local ip address of the current request.
    /// </summary>
    /// <value>
    /// The local ip address.
    /// </value>
    public string? LocalIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the remote ip address of the current request.
    /// </summary>
    /// <value>
    /// The remote ip address.
    /// </value>
    public string? RemoteIpAddress { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthEvent" /> class.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <exception cref="ArgumentNullException">category.</exception>
#pragma warning disable ACL1003 // Signature contains too many parameters
    protected AuthEvent(string category, string name, EventTypes type, int id)
#pragma warning restore ACL1003 // Signature contains too many parameters
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Name = name ?? throw new ArgumentNullException(nameof(name));

        EventType = type;
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthEvent" /> class.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException">category.</exception>
#pragma warning disable ACL1003 // Signature contains too many parameters
    protected AuthEvent(string category, string name, EventTypes type, int id, string? message)
#pragma warning restore ACL1003 // Signature contains too many parameters
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Name = name ?? throw new ArgumentNullException(nameof(name));

        EventType = type;
        Id = id;
        Message = message;
    }

    /// <summary>
    /// Prepares the current event for persistence.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the operation.</returns>
    public virtual Task PrepareAsync()
    {
        return Task.CompletedTask;
    }
}
