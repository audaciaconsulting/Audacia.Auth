using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Audacia.Auth.OpenIddict.Seeding;

/// <summary>
/// Base type for seeding a database with OpenIddict application and scope entities.
/// </summary>
public abstract class OpenIddictSeedBase<TKey> where TKey : IEquatable<TKey>
{
    private readonly string _identityProjectBasePath;
    private readonly string _identityProjectName;

    /// <summary>
    /// Initializes an instance of <see cref="OpenIddictSeedBase{TKey}"/>.
    /// </summary>
    /// <param name="identityProjectBasePath">The base path to the Identity project containing the relevant appsettings.json file.</param>
    /// <param name="identityProjectName">The name of the Identity project.</param>
    protected OpenIddictSeedBase(string identityProjectBasePath, string identityProjectName)
    {
        _identityProjectBasePath = identityProjectBasePath;
        _identityProjectName = identityProjectName;
    }

    /// <summary>
    /// Seeds the database with OpenIddict configuration.
    /// </summary>
    /// <returns></returns>
    public async Task SeedAsync()
    {
        var services = ConfigureServices();
        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<OpenIddictSeedingRunner>();
        await runner.RunAsync().ConfigureAwait(false);
    }

    private IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();
        var configuration = LoadConfiguration();
        var openIdConnectConfig = GetOpenIdConnectConfig(configuration);
        var openIddictBuilder = services
            .AddTransient<OpenIddictSeedingRunner>()
            .AddSingleton(openIdConnectConfig)
            .AddLogging(options => options.AddConsole())
            .AddOpenIddict()
            .AddCore();
        ConfigureAdditionalServices(services, configuration);
        ConfigureStores(openIddictBuilder);

        return services;
    }

    private IConfiguration LoadConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(_identityProjectBasePath)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

    private OpenIdConnectConfig GetOpenIdConnectConfig(IConfiguration configuration)
    {
        var identityProjectAssembly = Assembly.LoadFrom(Path.Combine(_identityProjectBasePath, $"{_identityProjectName}.dll"));
        var mapperTypes = identityProjectAssembly
            .GetTypes()
            .Where(type => typeof(IOpenIdConnectConfigMapper).IsAssignableFrom(type))
            .ToArray();
        if (!mapperTypes.Any())
        {
            // No implementation of IOpenIdConnectConfigMapper, so try and get the config object directly
            var openIdConnectConfig = configuration.GetSection("OpenIdConnectConfig").Get<OpenIdConnectConfig>();
            if (openIdConnectConfig is null)
            {
                throw new InvalidOperationException($"Either an implementation of '{nameof(IOpenIdConnectConfigMapper)}' must be provided in project {_identityProjectName}, or an 'OpenIdConnectConfig' section must be present in configuration that maps to an object of type '${nameof(OpenIdConnectConfig)}'.");
            }

            return openIdConnectConfig;
        }

        if (Activator.CreateInstance(mapperTypes.First()) is not IOpenIdConnectConfigMapper mapper)
        {
            throw new InvalidOperationException($"The implementation of '{nameof(IOpenIdConnectConfigMapper)}' in project {_identityProjectName} must have a parameterless constructor.");
        }

        return mapper.Map(configuration);
    }

    /// <summary>
    /// When overridden in a subclass, performs any additional dependency injection configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> in which to register services.</param>
    /// <param name="configuration">The current <see cref="IConfiguration"/> instance.</param>
    /// <returns>The given <paramref name="services"/>.</returns>
    protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    /// <summary>
    /// When overridden in a subclass, configures the stores to be used for seed data.
    /// </summary>
    /// <param name="builder">The <see cref="OpenIddictCoreBuilder"/> instance to which to add the stores.</param>
    /// <returns>The given <paramref name="builder"/>.</returns>
    protected abstract OpenIddictCoreBuilder ConfigureStores(OpenIddictCoreBuilder builder);
}
