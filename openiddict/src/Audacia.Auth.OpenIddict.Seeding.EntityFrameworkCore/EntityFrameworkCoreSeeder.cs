using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFrameworkCore
{
    /// <summary>
    /// Implementation of <see cref="OpenIddictSeedBase{TKey}"/> that uses Entity Framework Core stores.
    /// </summary>
    public class EntityFrameworkCoreSeeder<TKey> : OpenIddictSeedBase<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly string _connectionStringName;

        /// <summary>
        /// Initializes an instance of <see cref="EntityFrameworkCoreSeeder{TKey}"/>.
        /// </summary>
        /// <param name="identityProjectBasePath">The base path to the Identity project containing the relevant appsettings.json file.</param>
        /// <param name="identityProjectName">The name of the Identity project.</param>
        /// <param name="connectionStringName">The name of the connection string in config.</param>
        public EntityFrameworkCoreSeeder(string identityProjectBasePath, string identityProjectName, string connectionStringName) 
            : base(identityProjectBasePath, identityProjectName)
        {
            _connectionStringName = connectionStringName;
        }

        /// <inheritdoc />
        protected override IServiceCollection ConfigureAdditionalServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(_connectionStringName);
            services.AddDbContext<OpenIddictContext<TKey>>(options =>
            {
                options.UseSqlServer(connectionString);
                options.UseOpenIddict<TKey>();
            });

            return base.ConfigureAdditionalServices(services, configuration);
        }

        /// <inheritdoc />
        protected override OpenIddictCoreBuilder ConfigureStores(OpenIddictCoreBuilder builder)
        {
            builder
                .UseEntityFrameworkCore()
                .UseDbContext<OpenIddictContext<TKey>>()
                .ReplaceDefaultEntities<TKey>();

            return builder;
        }
    }
}