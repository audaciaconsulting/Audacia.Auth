using Microsoft.Extensions.Hosting;

namespace Audacia.Auth.OpenIddict.Seeding
{
    public class OpenIddictWorker : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}