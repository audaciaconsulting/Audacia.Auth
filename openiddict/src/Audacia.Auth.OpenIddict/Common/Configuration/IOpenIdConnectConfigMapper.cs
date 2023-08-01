using Microsoft.Extensions.Configuration;

namespace Audacia.Auth.OpenIddict.Common.Configuration;

/// <summary>
/// Represents a type that can map an <see cref="IConfiguration"/> object to an instance of <see cref="OpenIdConnectConfig"/>.
/// </summary>
public interface IOpenIdConnectConfigMapper
{
    /// <summary>
    /// Maps the given <paramref name="configuration"/> to an instance of <see cref="OpenIdConnectConfig"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> object to map.</param>
    /// <returns>An instance of <see cref="OpenIdConnectConfig"/>.</returns>
    OpenIdConnectConfig Map(IConfiguration configuration);
}
