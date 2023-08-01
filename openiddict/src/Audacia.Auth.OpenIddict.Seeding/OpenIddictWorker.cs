using System;
using System.Threading;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Seeding;

/// <summary>
/// Hosted service that can seed OpenIddict configuration data.
/// </summary>
public class OpenIddictWorker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes an instance of <see cref="OpenIddictWorker"/>.
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> instance.</param>
    public OpenIddictWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var configuration = scope.ServiceProvider.GetService<OpenIdConnectConfig>();
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<OpenIddictSeedingRunner>>();

        if (configuration != null)
        {
            var runner = new OpenIddictSeedingRunner(applicationManager, scopeManager, configuration, logger);
            await runner.RunAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}