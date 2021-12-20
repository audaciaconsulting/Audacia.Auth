using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Custom implementation of <see cref="OpenIddictApplicationManager{TApplication}"/> to provide hookpoints for certain functionality
    /// to be overridden, e.g. redirect uri validation.
    /// </summary>
    /// <typeparam name="TApplication">The type of the Application entity.</typeparam>
    public class CustomOpenIddictApplicationManager<TApplication> : OpenIddictApplicationManager<TApplication>
        where TApplication : class
    {
        private readonly IRedirectUriValidator _redirectUriValidator;

        /// <summary>
        /// Initializes an instance of <see cref="CustomOpenIddictApplicationManager{TApplication}"/>.
        /// </summary>
        /// <param name="cache">The instance of <see cref="IOpenIddictApplicationCache{TApplication}"/> to use.</param>
        /// <param name="logger">The instance of <see cref="ILogger{TCategoryName}"/> to use.</param>
        /// <param name="options">The instance of <see cref="IOptionsMonitor{TOptions}"/> to use.</param>
        /// <param name="resolver">The instance of <see cref="IOpenIddictApplicationStoreResolver"/> to use.</param>
        /// <param name="redirectUriValidator">The instance of <see cref="IRedirectUriValidator"/> to use.</param>
        [SuppressMessage("Maintainability", "ACL1003:Signature contains too many parameters", Justification = "Base class already has four parameters.")]
        public CustomOpenIddictApplicationManager(
        IOpenIddictApplicationCache<TApplication> cache,
        ILogger<OpenIddictApplicationManager<TApplication>> logger,
        IOptionsMonitor<OpenIddictCoreOptions> options,
        IOpenIddictApplicationStoreResolver resolver,
        IRedirectUriValidator redirectUriValidator)
            : base(cache, logger, options, resolver)
        {
            _redirectUriValidator = redirectUriValidator;
        }

        /// <inheritdoc />
        public override async ValueTask<bool> ValidateRedirectUriAsync(TApplication application, string address, CancellationToken cancellationToken = default)
        {
            var applicationDescriptor = new OpenIddictApplicationDescriptor();
            await PopulateAsync(applicationDescriptor, application, cancellationToken).ConfigureAwait(false);

            return await _redirectUriValidator.ValidateAsync(address, applicationDescriptor).ConfigureAwait(false);
        }
    }
}
