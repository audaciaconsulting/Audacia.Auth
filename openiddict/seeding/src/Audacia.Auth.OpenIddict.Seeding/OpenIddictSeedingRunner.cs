using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Audacia.Auth.OpenIddict.Seeding
{
    /// <summary>
    /// Class to run the seeding of OpenIddict configuration data.
    /// </summary>
    public class OpenIddictSeedingRunner
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly OpenIdConnectConfig _configuration;
        private readonly ILogger<OpenIddictSeedingRunner> _logger;

        /// <summary>
        /// Initializes an instance of <see cref="OpenIddictSeedingRunner"/>.
        /// </summary>
        /// <param name="applicationManager">The <see cref="IOpenIddictApplicationManager"/> instance to use.</param>
        /// <param name="scopeManager">The <see cref="IOpenIddictScopeManager"/> instance to use.</param>
        /// <param name="configuration">The <see cref="OpenIdConnectConfig"/> object representing the current configuration.</param>
        /// <param name="logger">An <see cref="ILogger"/> instance.</param>
        public OpenIddictSeedingRunner(
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager,
            OpenIdConnectConfig configuration,
            ILogger<OpenIddictSeedingRunner> logger)
        {
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously runs the seeding of OpenIddict data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            await RegisterApplicationsAsync().ConfigureAwait(false);
            await RegisterScopesAsync().ConfigureAwait(false);
            await CheckForDeletedApplicationsAsync().ConfigureAwait(false);
        }

        private async Task RegisterApplicationsAsync()
        {
            await RegisterUiClientsAsync(_configuration.UiClients).ConfigureAwait(false);
            await RegisterApiClientsAsync(_configuration.ApiClients).ConfigureAwait(false);
            await RegisterTestAutomationClientsAsync(_configuration.TestAutomationClients).ConfigureAwait(false);
        }

        private async Task RegisterUiClientsAsync(IEnumerable<UiClient>? clients)
        {
            if (clients is null)
            {
                return;
            }

            _logger.LogInformation("Registering UI clients.");
            foreach (var client in clients)
            {
                var applicationDescriptor = CreateUiClient(client);
                await SaveClientAsync(applicationDescriptor).ConfigureAwait(false);
            }
        }

        private static OpenIddictApplicationDescriptor CreateUiClient(UiClient clientConfig)
        {
            var applicationClient = new OpenIddictApplicationDescriptor
            {
                ClientId = clientConfig.ClientId,
                ConsentType = ConsentTypes.Implicit, // Whether the user needs to provide consent to resource access
                Type = ClientTypes.Public,
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
            if (clientConfig.RedirectUris == null)
            {
                applicationClient.RedirectUris.Add(clientConfig.BaseUrl);
            }
            else
            {
                foreach (var uri in clientConfig.RedirectUris)
                {
                    applicationClient.RedirectUris.Add(uri);
                }
            }

            if (clientConfig.ClientScopes != null)
            {
                // Define what client scopes this client can access, i.e. booking-api, finance-api
                foreach (var scope in clientConfig.ClientScopes)
                {
                    applicationClient.Permissions.Add(Permissions.Prefixes.Scope + scope);
                }
            }

            return applicationClient;
        }

        private async Task RegisterApiClientsAsync(IEnumerable<ApiClient>? clients)
        {
            if (clients == null)
            {
                return;
            }

            _logger.LogInformation("Registering API clients.");
            foreach (var client in clients)
            {
                var applicationDescriptor = CreateApiClient(client);
                await SaveClientAsync(applicationDescriptor).ConfigureAwait(false);
            }
        }

        private static OpenIddictApplicationDescriptor CreateApiClient(ApiClient clientConfig)
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
                    Permissions.GrantTypes.ClientCredentials
                }
            };

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

        private async Task RegisterTestAutomationClientsAsync(IEnumerable<TestAutomationClient>? clients)
        {
            if (clients == null)
            {
                return;
            }

            _logger.LogInformation("Registering Test Automation clients.");
            foreach (var client in clients)
            {
                var applicationDescriptor = CreateTestAutomationClient(client);
                await SaveClientAsync(applicationDescriptor).ConfigureAwait(false);
            }
        }

        private static OpenIddictApplicationDescriptor CreateTestAutomationClient(TestAutomationClient clientConfig)
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
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken
                }
            };

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

        private async ValueTask SaveClientAsync(OpenIddictApplicationDescriptor client)
        {
            if (client.ClientId is null)
            {
                return;
            }

            var existingClient = await _applicationManager.FindByClientIdAsync(client.ClientId).ConfigureAwait(false);
            if (existingClient == null)
            {
                _logger.LogInformation("Creating client '{ClientId}'.", client.ClientId);
                await _applicationManager.CreateAsync(client).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation("Updating client '{ClientId}'.", client.ClientId);
                await _applicationManager.UpdateAsync(existingClient, client).ConfigureAwait(false);
            }
        }

        private async Task RegisterScopesAsync()
        {
            if (_configuration.Scopes == null) { return; }

            _logger.LogInformation("Registering scopes.");
            foreach (var scopeConfig in _configuration.Scopes)
            {
                var applicationScope = CreateApplicationScope(scopeConfig);
                await SaveScopeAsync(scopeConfig, applicationScope).ConfigureAwait(false);
            }
        }

        private static OpenIddictScopeDescriptor CreateApplicationScope(OpenIdConnectScope scopeConfig)
        {
            var scope = new OpenIddictScopeDescriptor
            {
                Name = scopeConfig.Name
            };

            if (scopeConfig.Resources != null)
            {
                // Define the api clients that have permission to use this scope (can introspect / can provide data)
                foreach (var resource in scopeConfig.Resources)
                {
                    scope.Resources.Add(resource);
                }
            }

            return scope;
        }

        private async Task SaveScopeAsync(OpenIdConnectScope scopeConfig, OpenIddictScopeDescriptor applicationScope)
        {
            var existingScope = await _scopeManager.FindByNameAsync(scopeConfig.Name).ConfigureAwait(false);
            if (existingScope == null)
            {
                _logger.LogInformation("Creating scope '{Scope}'.", applicationScope.Name);
                await _scopeManager.CreateAsync(applicationScope).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation("Updating scope '{Scope}'.", applicationScope.Name);
                await _scopeManager.UpdateAsync(existingScope, applicationScope).ConfigureAwait(false);
            }
        }

        private async ValueTask CheckForDeletedApplicationsAsync()
        {
            _logger.LogInformation("Checking for deleted clients.");
            var deletedApplications = await GetDeletedApplicationsAsync().ConfigureAwait(false);
            foreach (var application in deletedApplications)
            {
                _logger.LogInformation("Deleting client '{Client}'.", application);
                await _applicationManager.DeleteAsync(application).ConfigureAwait(false);
            }
        }

        private async Task<IEnumerable<object>> GetDeletedApplicationsAsync()
        {
            var deletedApplications = new List<object>();
            await foreach (var application in _applicationManager.ListAsync())
            {
                var applicationDescriptor = new OpenIddictApplicationDescriptor();
                await _applicationManager.PopulateAsync(applicationDescriptor, application).ConfigureAwait(false);
                if (_configuration.AllClients.All(client => client.ClientId != applicationDescriptor.ClientId))
                {
                    deletedApplications.Add(application);
                }
            }

            return deletedApplications;
        }
    }
}
