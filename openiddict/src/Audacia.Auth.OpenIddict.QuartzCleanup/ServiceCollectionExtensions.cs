using System;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Audacia.Auth.OpenIddict.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Audacia.Auth.OpenIddict.QuartzCleanup
{
    /// <summary>
    /// Extensions to the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds OpenIddict services to the given <paramref name="services"/>.
        /// Automatic cleanup of expired tokens via a hosted service is also set up.
        /// </summary>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TId">The type of the user's primary key.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> object to which to add the services.</param>
        /// <param name="optionsBuilder">A delegate containing the additional OpenIddict configuration.</param>
        /// <param name="userIdGetter">A delegate that, when invoked, gets the ID of the given user.</param>
        /// <param name="openIdConnectConfig">An <see cref="OpenIdConnectConfig"/> object, which represents the configuration of the authorization server.</param>
        /// <param name="hostingEnvironment">The current <see cref="IWebHostEnvironment"/>.</param>
        /// <returns>An instance of <see cref="OpenIddictBuilder"/> to which further configuration can be performed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="openIdConnectConfig"/> is <see langword="null"/>.</exception>
        public static OpenIddictBuilder AddOpenIddictWithCleanup<TUser, TId>(
            this IServiceCollection services,
            Action<OpenIddictCoreBuilder> optionsBuilder,
            Func<TUser, TId> userIdGetter,
            OpenIdConnectConfig openIdConnectConfig,
            IWebHostEnvironment hostingEnvironment)
                where TUser : class
                where TId : IEquatable<TId>
        {
            Action<OpenIddictCoreBuilder> quartzOptions = builder => builder.UseQuartz();
            Action<OpenIddictCoreBuilder> combinedBuilder = optionsBuilder + quartzOptions;

            return services
                .AddQuartz(options =>
                {
                    options.UseMicrosoftDependencyInjectionJobFactory();
                    options.UseSimpleTypeLoader();
                    options.UseInMemoryStore();
                })
                .AddQuartzHostedService(options => options.WaitForJobsToComplete = true)
                .AddOpenIddict(combinedBuilder, userIdGetter, openIdConnectConfig, hostingEnvironment);
        }
    }
}