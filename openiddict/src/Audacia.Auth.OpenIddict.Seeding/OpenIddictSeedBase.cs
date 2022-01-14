using System;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Audacia.Auth.OpenIddict.Seeding
{
    /// <summary>
    /// Base type for 
    /// </summary>
    public abstract class OpenIddictSeedBase<TKey> where TKey : IEquatable<TKey>
    {
        private readonly string _appSettingsBasePath;
        private readonly string _configSectionName;

        /// <summary>
        /// Initializes an instance of <see cref="OpenIddictSeedBase{TKey}"/>.
        /// </summary>
        /// <param name="appSettingsBasePath">The base path to the appsettings.json file.</param>
        /// <param name="configSectionName">The name of the config section containing the OpenIdConnect config.</param>
        protected OpenIddictSeedBase(string appSettingsBasePath, string configSectionName)
        {
            _appSettingsBasePath = appSettingsBasePath;
            _configSectionName = configSectionName;
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
                .SetBasePath(_appSettingsBasePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

        private OpenIdConnectConfig GetOpenIdConnectConfig(IConfiguration configuration)
        {
            return configuration.GetSection(_configSectionName).Get<OpenIdConnectConfig>();
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
}
