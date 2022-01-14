using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFramework
{
    /// <summary>
    /// Implementation of <see cref="OpenIddictSeedBase{TKey}"/> that uses Entity Framework 6.x stores.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class EntityFrameworkSeeder<TKey> : OpenIddictSeedBase<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly string _connectionStringName;

        /// <summary>
        /// Initializes an instance of <see cref="EntityFrameworkSeeder{TKey}"/>.
        /// </summary>
        /// <param name="appSettingsBasePath">The base path to the appsettings.json file.</param>
        /// <param name="configSectionName">The name of the config section containing the OpenIdConnect config.</param>
        /// <param name="connectionStringName">The name of the connection string in config.</param>
        public EntityFrameworkSeeder(string appSettingsBasePath, string configSectionName, string connectionStringName)
            : base(appSettingsBasePath, configSectionName)
        {
            _connectionStringName = connectionStringName;
        }

        /// <inheritdoc />
        protected override IServiceCollection ConfigureAdditionalServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(_connectionStringName);
            services.AddScoped(_ => new OpenIddictContext<TKey>(connectionString));

            return base.ConfigureAdditionalServices(services, configuration);
        }

        /// <inheritdoc />
        protected override OpenIddictCoreBuilder ConfigureStores(OpenIddictCoreBuilder builder)
        {
            var entityFrameworkBuilder = builder
                .UseEntityFramework()
                .UseDbContext<OpenIddictContext<TKey>>();

            if (typeof(TKey) == typeof(int))
            {
                OpenIddict.EntityFramework.IntKey.OpenIddictEntityFrameworkBuilderExtensions.ReplaceDefaultEntities(entityFrameworkBuilder);
            }
            else if (typeof(TKey) == typeof(Guid))
            {
                OpenIddict.EntityFramework.GuidKey.OpenIddictEntityFrameworkBuilderExtensions.ReplaceDefaultEntities(entityFrameworkBuilder);
            }

            return builder;
        }
    }
}
