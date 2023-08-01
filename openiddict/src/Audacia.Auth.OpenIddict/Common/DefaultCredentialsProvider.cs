using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server;

namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Default implementation of <see cref="ISigningCredentialsProvider"/> and <see cref="IEncryptionCredentialsProvider"/> that gets credentials from configuration.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
internal class DefaultCredentialsProvider : ISigningCredentialsProvider, IEncryptionCredentialsProvider
{
    private readonly OpenIddictServerOptions _serverOptions;

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultCredentialsProvider"/>.
    /// </summary>
    /// <param name="serverOptions">The options object containing the configured credentials.</param>
    /// <exception cref="ArgumentNullException">When the given <paramref name="serverOptions"/> is <see langword="null"/>.</exception>
    public DefaultCredentialsProvider(IOptions<OpenIddictServerOptions> serverOptions)
    {
        if (serverOptions == null)
        {
            throw new ArgumentNullException(nameof(serverOptions));
        }

        _serverOptions = serverOptions.Value;
    }

    /// <inheritdoc />
    public EncryptingCredentials GetEncryptionCredentials() => _serverOptions.EncryptionCredentials.First();

    /// <inheritdoc />
    public SigningCredentials GetSigningCredentials() => _serverOptions.SigningCredentials.First();
}
