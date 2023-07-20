using Microsoft.Extensions.DependencyInjection;

namespace Audacia.Auth.OpenIddict.Seeding;

/// <summary>
/// Extensions to the <see cref="IServiceCollection"/> type.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a hosted service to perform seeding of the data.
    /// Note this method should only be called when running the application locally.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> object to which to add the hosted service.</param>
    /// <returns>The given <paramref name="services"/> with the hosted service added.</returns>
    public static IServiceCollection AddLocalSeeding(this IServiceCollection services)
    {
        return services.AddHostedService<OpenIddictWorker>();
    }
}
