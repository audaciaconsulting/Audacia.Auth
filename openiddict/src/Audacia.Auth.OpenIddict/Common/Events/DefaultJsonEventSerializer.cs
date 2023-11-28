using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Audacia.Auth.OpenIddict.Common.Events;

/// <summary>
/// Default implementation of <see cref="IEventSerializer"/> that uses System.Text.Json to serialize events.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
internal class DefaultJsonEventSerializer : IEventSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    static DefaultJsonEventSerializer()
    {
        Options.Converters.Add(new JsonStringEnumConverter());
    }

    /// <inheritdoc />
    public string Serialize(AuthEvent authEvent) => JsonSerializer.Serialize(authEvent, Options);
}
