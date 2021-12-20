using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Default implementation of <see cref="IRedirectUriValidator"/> that checks the configured redirect uris of the client.
    /// </summary>
    public class DefaultRedirectUriValidator : IRedirectUriValidator
    {
        private readonly ILogger<DefaultRedirectUriValidator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRedirectUriValidator"/> class.
        /// </summary>
        /// <param name="logger">The instance of <see cref="ILogger{TCategoryName}"/> to use.</param>
        public DefaultRedirectUriValidator(ILogger<DefaultRedirectUriValidator> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public virtual Task<bool> ValidateAsync(string address, OpenIddictApplicationDescriptor client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            foreach (var uri in client.RedirectUris)
            {
                // Note: the redirect_uri must be compared using case-sensitive "Simple String Comparison".
                // See http://openid.net/specs/openid-connect-core-1_0.html#AuthRequest for more information.
                if (string.Equals(uri.ToString(), address, StringComparison.Ordinal))
                {
                    return Task.FromResult(true);
                }
            }

            _logger.LogInformation("The redirect uri '{Address}' for client '{ClientId}' is invalid.", address, client.ClientId);

            return Task.FromResult(false);
        }
    }
}
