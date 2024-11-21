using System;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Audacia.Auth.OpenIddict.DependencyInjection;
using Audacia.Auth.OpenIddict.QuartzCleanup.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Audacia.Auth.OpenIddict.QuartzCleanup;

/// <summary>
/// Extensions to the <see cref="IServiceCollection"/> type.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly TimeSpan DefaultMinimumAgeToCleanup = TimeSpan.FromHours(6);

    /// <summary>
    /// Adds OpenIddict services to the given <paramref name="services"/>.
    /// Automatic cleanup of expired tokens via a hosted service is also set up.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TId">The type of the user's primary key.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> object to which to add the services.</param>
    /// <param name="optionsBuilder">A delegate containing the additional OpenIddict configuration.</param>
    /// <param name="userIdGetter">A delegate that, when invoked, gets the ID of the given user.</param>
    /// <param name="openIdConnectConfig">An instance of <see cref="OpenIdConnectConfig"/>.</param>
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
            where TId : IEquatable<TId> =>
        services.AddOpenIddictWithCleanup<TUser, TId>(
            optionsBuilder,
            userIdGetter,
            openIdConnectConfig,
            hostingEnvironment,
            DefaultMinimumAgeToCleanup);

    /// <summary>
    /// Adds OpenIddict services to the given <paramref name="services"/>.
    /// Automatic cleanup of expired tokens via a hosted service is also set up.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TId">The type of the user's primary key.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> object to which to add the services.</param>
    /// <param name="optionsBuilder">A delegate containing the additional OpenIddict configuration.</param>
    /// <param name="userIdGetter">A delegate that, when invoked, gets the ID of the given user.</param>
    /// <param name="openIdConnectConfig">An instance of <see cref="OpenIdConnectCleanupConfig"/>, which contains an additional property to specify the minimum age of tokens to cleanup.</param>
    /// <param name="hostingEnvironment">The current <see cref="IWebHostEnvironment"/>.</param>
    /// <returns>An instance of <see cref="OpenIddictBuilder"/> to which further configuration can be performed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="openIdConnectConfig"/> is <see langword="null"/>.</exception>
    public static OpenIddictBuilder AddOpenIddictWithCleanup<TUser, TId>(
        this IServiceCollection services,
        Action<OpenIddictCoreBuilder> optionsBuilder,
        Func<TUser, TId> userIdGetter,
        OpenIdConnectCleanupConfig openIdConnectConfig,
        IWebHostEnvironment hostingEnvironment)
            where TUser : class
            where TId : IEquatable<TId>
    {
        if (openIdConnectConfig is null) throw new ArgumentNullException(nameof(openIdConnectConfig));

        var minimumAgeToCleanup = openIdConnectConfig.MinimumAgeToCleanup is null
            ? DefaultMinimumAgeToCleanup
            : openIdConnectConfig.MinimumAgeToCleanup.GetLifetime();

        return services.AddOpenIddictWithCleanup<TUser, TId>(
            optionsBuilder,
            userIdGetter,
            openIdConnectConfig,
            hostingEnvironment,
            minimumAgeToCleanup);
    }

    [CodeAnalysis.Analyzers.Helpers.ParameterCount.MaxParameterCount(5, Justification = "Code is clear enough with 5 params.")]
    private static OpenIddictBuilder AddOpenIddictWithCleanup<TUser, TId>(
        this IServiceCollection services,
        Action<OpenIddictCoreBuilder> optionsBuilder,
        Func<TUser, TId> userIdGetter,
        OpenIdConnectConfig openIdConnectConfig,
        IWebHostEnvironment hostingEnvironment,
        TimeSpan minimumAgeToCleanup)
            where TUser : class
            where TId : IEquatable<TId>
    {
        Action<OpenIddictCoreBuilder> quartzOptions = builder => builder.UseQuartz(options =>
        {
            options.SetMinimumAuthorizationLifespan(minimumAgeToCleanup);
            options.SetMinimumTokenLifespan(minimumAgeToCleanup);
        });
        Action<OpenIddictCoreBuilder> combinedBuilder = optionsBuilder + quartzOptions;

        return services
            .AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            })
            .AddQuartzHostedService(options => options.WaitForJobsToComplete = true)
            .AddOpenIddict(combinedBuilder, userIdGetter, openIdConnectConfig, hostingEnvironment);
    }
}