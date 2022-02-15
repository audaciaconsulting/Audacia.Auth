namespace Audacia.Auth.OpenIddict.Common.Events
{
    /// <summary>
    /// Represents a type that can serialize events.
    /// </summary>
    public interface IEventSerializer
    {
        /// <summary>
        /// Serializes the given <paramref name="authEvent"/>.
        /// </summary>
        /// <param name="authEvent">The <see cref="AuthEvent"/> to serialize.</param>
        /// <returns>A serialized version of the given <paramref name="authEvent"/>.</returns>
        string Serialize(AuthEvent authEvent);
    }
}
