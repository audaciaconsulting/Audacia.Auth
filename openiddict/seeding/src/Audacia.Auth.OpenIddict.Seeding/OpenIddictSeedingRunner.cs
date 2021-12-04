using System;
using System.Collections.Generic;
using System.Text;

namespace Audacia.Auth.OpenIddict.Seeding
{
    public static class OpenIddictSeedingRunner
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenIddictWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async Task RunAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var configuration = scope.ServiceProvider.GetService<OpenIdConnectConfig>();
            var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            if (configuration != null)
            {
                await RegisterApplicationsAsync(applicationManager, configuration);
                await RegisterScopesAsync(scopeManager, configuration);
            }
        }

        private static async Task RegisterApplicationsAsync(IOpenIddictApplicationManager manager, OpenIdConnectConfig config)
        {
            if (config.Clients is null) { return; }

            foreach (var clientConfig in config.Clients)
            {
                if (await manager.FindByClientIdAsync(clientConfig.ClientId) is null)
                {
                    OpenIddictApplicationDescriptor client;
                    if (clientConfig.GrantType == OAuthGrantType.AuthorizationCode)
                    {
                        client = CreateApplicationUserClient(clientConfig);
                    }
                    else
                    {
                        client = CreateApplicationResourceClient(clientConfig);
                    }

                    await manager.CreateAsync(client);
                }
            }
        }

        private static async Task CreateUiClientsAsync(IEnumerable<UiClient> clients, IOpenIddictApplicationManager manager)
        {
            if (clients is null)
            {
                return;
            }

            foreach (var client in clients)
            {
                var applicationDescriptor = CreateUiClient(client);
                await manager.CreateAsync(applicationDescriptor);
            }
        }

        /// <summary>
        /// Creates an application client for use by end users.
        /// </summary>
        private static OpenIddictApplicationDescriptor CreateUiClient(UiClient clientConfig)
        {
            var applicationClient = new OpenIddictApplicationDescriptor
            {
                ClientId = clientConfig.ClientId,
                ConsentType = ConsentTypes.Implicit, // Whether the user needs to provide consent to resource access
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Revocation,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            };

            // Attach URLs that the user can be redirected to after logout
            applicationClient.PostLogoutRedirectUris.Add(clientConfig.BaseUrl);

            // Attach URLs required for authentication, i.e. auth-callbacks, silent renew
            foreach (var uri in clientConfig.RedirectUris)
            {
                applicationClient.RedirectUris.Add(uri);
            }

            // Define what client scopes this client can access, i.e. booking-api, finance-api
            foreach (var scope in clientConfig.ClientScopes)
            {
                applicationClient.Permissions.Add(Permissions.Prefixes.Scope + scope);
            }

            return applicationClient;
        }

        /// <summary>
        /// Creates an application client for use by API (resource servers).
        /// </summary>
        private static OpenIddictApplicationDescriptor CreateApplicationResourceClient(OpenIdConnectClientBase clientConfig)
        {
            var applicationClient = new OpenIddictApplicationDescriptor
            {
                ClientId = clientConfig.ClientId,
                ClientSecret = clientConfig.ClientSecret,
                ConsentType = ConsentTypes.Implicit,
                Type = ClientTypes.Confidential,
                Permissions =
                {
                    // Minimum requirement for a resource server
                    Permissions.Endpoints.Introspection,
                    // Required for resources that also interact with the identity server
                    Permissions.Endpoints.Token,
                    clientConfig.GrantType == OAuthGrantType.ClientCredentials
                        ? Permissions.GrantTypes.ClientCredentials
                        : Permissions.GrantTypes.Password
                }
            };

            if (clientConfig.GrantType == OAuthGrantType.ResourceOwnerPasswordCredential)
            {
                applicationClient.Permissions.Add(Permissions.GrantTypes.RefreshToken);
            }

            // Attach what client scopes can be inspected by this resource client, i.e. booking-api, finance-api
            if (clientConfig.ClientScopes != null)
            {
                foreach (var scope in clientConfig.ClientScopes)
                {
                    applicationClient.Permissions.Add(Permissions.Prefixes.Scope + scope);
                }
            }

            return applicationClient;
        }

        private static OpenIddictScopeDescriptor CreateApplicationScope(IdentityServerScope scopeConfig)
        {
            var scope = new OpenIddictScopeDescriptor
            {
                Name = scopeConfig.Name
            };

            // Define the api clients that have permission to use this scope (can introspect / can provide data)
            foreach (var resource in scopeConfig.Resources)
            {
                scope.Resources.Add(resource);
            }

            return scope;
        }

        private static async Task RegisterScopesAsync(IOpenIddictScopeManager manager, IdentityServerConfig config)
        {
            if (config.Scopes is null) { return; }

            foreach (var scopeConfig in config.Scopes)
            {
                if (await manager.FindByNameAsync(scopeConfig.Name) is null)
                {
                    var applicationScope = CreateApplicationScope(scopeConfig);
                    await manager.CreateAsync(applicationScope);
                }
            }
        }
    }
}
