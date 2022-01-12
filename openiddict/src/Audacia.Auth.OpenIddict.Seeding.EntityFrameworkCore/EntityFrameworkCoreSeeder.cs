﻿using System;
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
        /// <param name="appSettingsFilepath">The path to the appsettings.json file containing the OpenIdConnect config.</param>
        /// <param name="configSectionName">The name of the config section containing the OpenIdConnect config.</param>
        /// <param name="connectionStringName">The name of the connection string in config.</param>
        public EntityFrameworkCoreSeeder(string appSettingsFilepath, string configSectionName, string connectionStringName) 
            : base(appSettingsFilepath, configSectionName)
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