using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Audacia.Auth.OpenIddict.Authorize;
using Audacia.Auth.OpenIddict.Common;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Audacia.Auth.OpenIddict.Common.Extensions;
using Audacia.Auth.OpenIddict.Token;
using Audacia.Auth.OpenIddict.UserInfo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Audacia.Auth.OpenIddict.DependencyInjection
{
    /// <summary>
    /// Extensions to the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the given <typeparamref name="TProvider"/> to the dependency injection container
        /// as the implementation of <see cref="IAdditionalClaimsProvider{TUser}"/>.
        /// </summary>
        /// <typeparam name="TProvider">The type of <see cref="IAdditionalClaimsProvider{TUser}"/> implementation.</typeparam>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to which to add the <typeparamref name="TProvider"/>.</param>
        /// <returns>The given <paramref name="services"/>.</returns>
        public static IServiceCollection AddAdditionalClaimsProvider<TProvider, TUser>(this IServiceCollection services)
            where TProvider : class, IAdditionalClaimsProvider<TUser>
            where TUser : class => services.AddTransient<IAdditionalClaimsProvider<TUser>, TProvider>();

        /// <summary>
        /// Adds the given <typeparamref name="TService"/> to the dependency injection container
        /// as the implementation of <see cref="IProfileService{TUser}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the <see cref="IProfileService{TUser}"/> implementation.</typeparam>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to which to add the <typeparamref name="TService"/>.</param>
        /// <returns>The given <paramref name="services"/>.</returns>
        public static IServiceCollection AddProfileService<TService, TUser>(this IServiceCollection services)
            where TService : class, IProfileService<TUser>
            where TUser : class => services.AddTransient<IProfileService<TUser>, TService>();

        /// <summary>
        /// Adds the given <typeparamref name="THandler"/> to the dependency injection container
        /// as the implementation of <see cref="IPostAuthenticateHandler{TUser, TId}"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of the <see cref="IPostAuthenticateHandler{TUser, TId}"/> implementation.</typeparam>
        /// <typeparam name="TUser">The type of user.</typeparam>
        /// <typeparam name="TId">The type of the user's primary key.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to which to add the <typeparamref name="THandler"/>.</param>
        /// <returns>The given <paramref name="services"/>.</returns>
        public static IServiceCollection AddPostAuthenticateHandler<THandler, TUser, TId>(this IServiceCollection services)
            where THandler : class, IPostAuthenticateHandler<TUser, TId>
            where TUser : class
            where TId : IEquatable<TId> => services.AddTransient<IPostAuthenticateHandler<TUser, TId>, THandler>();

        /// <summary>
        /// Adds OpenIddict services to the given <paramref name="services"/>.
        /// </summary>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TId">The type of the user's primary key.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> object to which to add the services.</param>
        /// <param name="optionsBuilder">A delegate containing the additional OpenIddict configuration.</param>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> representing the current configuration.</param>
        /// <param name="userIdGetter">A delegate that, when invoked, gets the ID for a given user.</param>
        /// <param name="openIdConnectConfigMapper">An instance of <see cref="IOpenIdConnectConfigMapper"/> which can map to an <see cref="OpenIdConnectConfig"/> object.</param>
        /// <param name="hostingEnvironment">The current <see cref="IWebHostEnvironment"/>.</param>
        /// <returns>An instance of <see cref="OpenIddictBuilder"/> to which further configuration can be performed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="openIdConnectConfigMapper"/> is <see langword="null"/>.</exception>
        [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "Needs all parameters.")]
        public static OpenIddictBuilder AddOpenIddict<TUser, TId>(
            this IServiceCollection services,
            Action<OpenIddictCoreBuilder> optionsBuilder,
            IConfiguration configuration,
            Func<TUser, TId> userIdGetter,
            IOpenIdConnectConfigMapper openIdConnectConfigMapper,
            IWebHostEnvironment hostingEnvironment)
            where TUser : class
            where TId : IEquatable<TId>
        {
            if (openIdConnectConfigMapper == null) throw new ArgumentNullException(nameof(openIdConnectConfigMapper));

            UserWrapper<TUser, TId>.UserIdGetter = userIdGetter;
            var openIdConnectConfig = openIdConnectConfigMapper.Map(configuration);

            return services
                .AddServices<TUser, TId>()
                .AddSingleton(openIdConnectConfig)
                .ConfigureOpenIddict(optionsBuilder, openIdConnectConfig, hostingEnvironment);
        }

        private static IServiceCollection AddServices<TUser, TId>(this IServiceCollection services)
            where TUser : class
            where TId : IEquatable<TId>
        {
            return services
                .AddAdditionalClaimsProvider<DefaultAdditionalClaimsProvider<TUser>, TUser>()
                .AddProfileService<DefaultProfileService<TUser>, TUser>()
                .AddPostAuthenticateHandler<DefaultPostAuthenticateHandler<TUser, TId>, TUser, TId>()
                .AddTransient<IAuthenticateResultHandler<TUser, TId>, DefaultAuthenticateResultHandler<TUser, TId>>()
                .AddTransient<IGetTokenHandler, DefaultGetTokenHandler>()
                .AddTransient<IUserInfoHandler<TUser, TId>, DefaultUserInfoHandler<TUser, TId>>()
                .AddTransient<IClaimsPrincipalProviderFactory, ClaimsPrincipalProviderFactory<TUser, TId>>()
                .AddTransient<ClientCredentialsClaimPrincipalProvider>()
                .AddTransient<PasswordClaimsPrincipalProvider<TUser, TId>>()
                .AddTransient<CodeExchangeClaimsPrincipalProvider<TUser>>();
        }

        private static OpenIddictBuilder ConfigureOpenIddict(
            this IServiceCollection services,
            Action<OpenIddictCoreBuilder> optionsBuilder,
            OpenIdConnectConfig openIdConnectConfig,
            IWebHostEnvironment hostingEnvironment)
        {
            return services
                .AddOpenIddict()
                .AddCore(optionsBuilder)
                .ConfigureOpenIddictServer(openIdConnectConfig, hostingEnvironment)
                .ConfigureOpenIddictValidation(openIdConnectConfig);
        }

        private static OpenIddictBuilder ConfigureOpenIddictServer(
            this OpenIddictBuilder openIddictBuilder,
            OpenIdConnectConfig openIdConnectConfig,
            IWebHostEnvironment hostingEnvironment)
        {
            return openIddictBuilder.AddServer(options =>
            {
                AddEndpoints(options);
                AddFlows(options, openIdConnectConfig);
                AddScopes(openIdConnectConfig, options);
                SetDefaultTokenLifetimes(options);
                ConfigureSigningCredential(options, openIdConnectConfig, hostingEnvironment);
                AddAspNetCore(options, openIdConnectConfig);
            });
        }

        private static void AddEndpoints(OpenIddictServerBuilder options)
        {
            // Enable endpoints, these need to be explicitly enabled
            options.SetAuthorizationEndpointUris("/connect/authorize")
                .SetLogoutEndpointUris("/account/logout")
                .SetIntrospectionEndpointUris("/connect/introspect")
                .SetTokenEndpointUris("/connect/token")
                .SetUserinfoEndpointUris("/connect/userinfo");
        }

        private static void AddFlows(OpenIddictServerBuilder options, OpenIdConnectConfig openIdConnectConfig)
        {
            if (openIdConnectConfig.UiClients?.Any() == true)
            {
                options.AllowAuthorizationCodeFlow()
                    .RequireProofKeyForCodeExchange();
            }
            
            if (openIdConnectConfig.ApiClients?.Any() == true)
            {
                options.AllowClientCredentialsFlow();
            }
                
            if (openIdConnectConfig.TestAutomationClients?.Any() == true)
            {
                options.AllowPasswordFlow();
            }

            options.AllowRefreshTokenFlow();
        }

        private static void AddScopes(OpenIdConnectConfig openIdConnectConfig, OpenIddictServerBuilder options)
        {
            // Get configured scopes i.e. "email", "profile", "roles", "bookings-api"...
            var configurableScopes = openIdConnectConfig.AllClients
                .SelectMany(c => c.ClientScopes.EmptyIfNull())
                .Concat(new[]
                { // default required scopes
                        Scopes.Profile,
                        Scopes.Email,
                        Scopes.Roles
                })
                .Distinct()
                .ToArray();
            options.RegisterScopes(configurableScopes);
        }

        private static void SetDefaultTokenLifetimes(OpenIddictServerBuilder options)
        {
            // Globally set the lifetime of your tokens, individual token lifetimes per application resource must be done in the AuthorisationController
            options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
            options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));
        }

        private static void AddAspNetCore(OpenIddictServerBuilder builder, OpenIdConnectConfig openIdConnectConfig)
        {
            // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
            builder
                .Configure(options => options.Issuer = openIdConnectConfig.Url)
                .UseAspNetCore()
                .EnableStatusCodePagesIntegration()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableTokenEndpointPassthrough()
                .EnableUserinfoEndpointPassthrough();
        }

        private static OpenIddictBuilder ConfigureOpenIddictValidation(this OpenIddictBuilder openIddictBuilder, OpenIdConnectConfig openIdConnectConfig)
        {
            return openIddictBuilder.AddValidation(options =>
            {
                // Configure the audience accepted by this resource server.
                // The value MUST match the audience associated with the resource scope.
                var audiences = openIdConnectConfig!.AllClients!.Select(x => x.ClientId).ToArray();
                options.AddAudiences(audiences);
                options.UseLocalServer();
                options.UseDataProtection();
                options.UseAspNetCore();
            });
        }

#pragma warning disable ACL1002 // Member or local function contains too many statements
        private static OpenIddictServerBuilder ConfigureSigningCredential(
            OpenIddictServerBuilder openIddictServerBuilder,
            OpenIdConnectConfig identityServerConfig,
            IWebHostEnvironment hostingEnvironment)
#pragma warning restore ACL1002 // Member or local function contains too many statements
        {
            if (hostingEnvironment.IsDevelopment())
            {
                return openIddictServerBuilder
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate()
                    // Allow the access token to be read by the developer during local development
                    // this will cause unexpected behaviour in prod if we rely on manual token inspection
                    .DisableAccessTokenEncryption();
            }

            if (identityServerConfig.EncryptionCertificateThumbprint == identityServerConfig.SigningCertificateThumbprint)
            {
                throw new OpenIddictConfigurationException("The certificates used for token encryption and token signing should not be the same.");
            }

            var encryptionCertificate = FindCertificate(identityServerConfig.EncryptionCertificateThumbprint);
            if (encryptionCertificate == null)
            {
                throw new OpenIddictConfigurationException("Unable to load token encryption certificate");
            }

            var signingCertificate = FindCertificate(identityServerConfig.SigningCertificateThumbprint);
            if (signingCertificate == null)
            {
                throw new OpenIddictConfigurationException("Unable to load token signing certificate");
            }

            return openIddictServerBuilder
                .AddEncryptionCertificate(encryptionCertificate)
                .AddSigningCertificate(signingCertificate);
        }

        private static X509Certificate2? FindCertificate(string? certificateThumbprint)
        {
            if (certificateThumbprint == null)
            {
                return null;
            }

            X509Certificate2? certificate = null;
            using (var certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certificateStore.Open(OpenFlags.ReadOnly);
                var certificateCollection = certificateStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    certificateThumbprint,
                    false);
                if (certificateCollection.Count > 0)
                {
                    certificate = certificateCollection[0];
                }
            }

            return certificate;
        }
    }
}
