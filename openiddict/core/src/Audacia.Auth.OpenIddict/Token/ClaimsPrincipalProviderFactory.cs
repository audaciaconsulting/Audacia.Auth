using System;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <inheritdoc />
    public class ClaimsPrincipalProviderFactory<TUser, TId> : IClaimsPrincipalProviderFactory
        where TUser : class
    {
        private readonly ClientCredentialsClaimPrincipalProvider _clientCredentialsClaimPrincipalProvider;
        private readonly PasswordClaimsPrincipalProvider<TUser, TId> _passwordClaimsPrincipalProvider;
        private readonly CodeExchangeClaimsPrincipalProvider<TUser> _codeExchangeClaimsPrincipalProvider;

        /// <summary>
        /// Initializes an instance of <see cref="ClaimsPrincipalProviderFactory{TUser, TId}"/>.
        /// </summary>
        /// <param name="clientCredentialsClaimPrincipalProvider">The <see cref="IClaimsPrincipalProvider"/> for the client credentials flow.</param>
        /// <param name="passwordClaimsPrincipalProvider">The <see cref="IClaimsPrincipalProvider"/> for the resource owner password credential flow.</param>
        /// <param name="codeExchangeClaimsPrincipalProvider">The <see cref="IClaimsPrincipalProvider"/> for the exchanging a code for a token.</param>
        public ClaimsPrincipalProviderFactory(
            ClientCredentialsClaimPrincipalProvider clientCredentialsClaimPrincipalProvider,
            PasswordClaimsPrincipalProvider<TUser, TId> passwordClaimsPrincipalProvider,
            CodeExchangeClaimsPrincipalProvider<TUser> codeExchangeClaimsPrincipalProvider)
        {
            _clientCredentialsClaimPrincipalProvider = clientCredentialsClaimPrincipalProvider;
            _passwordClaimsPrincipalProvider = passwordClaimsPrincipalProvider;
            _codeExchangeClaimsPrincipalProvider = codeExchangeClaimsPrincipalProvider;
        }

        /// <inheritdoc />
        public IClaimsPrincipalProvider CreateProvider(OpenIddictRequest openIddictRequest)
        {
            if (openIddictRequest.IsPasswordGrantType())
            {
                return _passwordClaimsPrincipalProvider;
            }

            if (openIddictRequest.IsAuthorizationCodeGrantType() ||
                     openIddictRequest.IsDeviceCodeGrantType() ||
                     openIddictRequest.IsRefreshTokenGrantType())
            {
                return _codeExchangeClaimsPrincipalProvider;
            }

            if (openIddictRequest.IsClientCredentialsGrantType())
            {
                return _clientCredentialsClaimPrincipalProvider;
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }
    }
}
