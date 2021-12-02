﻿using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Audacia.Auth.OpenIddict.Authorize;
using Audacia.Auth.OpenIddict.Common;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Audacia.Auth.OpenIddict.Token;
using Audacia.Auth.OpenIddict.UserInfo;
using Microsoft.AspNetCore.Authentication;
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
        /// Adds OpenIddict services to the given <paramref name="services"/>.
        /// </summary>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> object to which to add the services.</param>
        /// <param name="optionsBuilder">A delegate containing the additional OpenIddict configuration.</param>
        /// <param name="configuration">The current <see cref="IConfiguration"/> object.</param>
        /// <param name="hostingEnvironment">The current <see cref="IWebHostEnvironment"/>.</param>
        /// <returns>An instance of <see cref="OpenIddictBuilder"/> to which further configuration can be performed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <see langword="null"/>.</exception>
        public static OpenIddictBuilder AddOpenIddict<TUser, TKey>(
            this IServiceCollection services,
            Action<OpenIddictCoreBuilder> optionsBuilder,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            return services
                .AddServices<TUser, TKey>()
                .ConfigureOpenIddict(optionsBuilder, configuration, hostingEnvironment);
        }

        private static IServiceCollection AddServices<TUser, TKey>(this IServiceCollection services)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return services
                .AddTransient<IAdditionalClaimsProvider<TUser, TKey>, DefaultAdditionalClaimsProvider<TUser, TKey>>()
                .AddTransient<AuthenticateResultHandler<TUser, TKey>>()
                .AddTransient<GetTokenHandler>()
                .AddTransient<UserInfoHandler<TUser, TKey>>()
                .AddTransient<IClaimsPrincipalProviderFactory, ClaimsPrincipalProviderFactory<TUser, TKey>>()
                .AddTransient<ClientCredentialsClaimPrincipalProvider>()
                .AddTransient<PasswordClaimsPrincipalProvider<TUser, TKey>>()
                .AddTransient<CodeExchangeClaimsPrincipalProvider<TUser, TKey>>()
                .AddScoped<IClaimsTransformation, ClaimsTransformation<TUser, TKey>>();
        }

        private static OpenIddictBuilder ConfigureOpenIddict(
            this IServiceCollection services,
            Action<OpenIddictCoreBuilder> optionsBuilder,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment)
        {
            var openIdConnectConfig = configuration
                .GetSection(nameof(OpenIdConnectConfig))
                .Get<OpenIdConnectConfig>();

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
            if (openIdConnectConfig.UiClients.Any())
            {
                options.AllowAuthorizationCodeFlow()
                    .RequireProofKeyForCodeExchange();
            }
            
            if (openIdConnectConfig.ApiClients.Any())
            {
                options.AllowClientCredentialsFlow();
            }
                
            if (openIdConnectConfig.TestAutomationClients.Any())
            {
                options.AllowPasswordFlow();
            }

            options.AllowRefreshTokenFlow();
        }

        private static void AddScopes(OpenIdConnectConfig openIdConnectConfig, OpenIddictServerBuilder options)
        {
            // Get configured scopes i.e. "email", "profile", "roles", "bookings-api"...
            var configurableScopes = openIdConnectConfig.AllClients
                .SelectMany(c => c.ClientScopes)
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
