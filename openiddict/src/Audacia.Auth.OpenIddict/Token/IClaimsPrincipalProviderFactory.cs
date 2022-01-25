using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token
{
    /// <summary>
    /// Represents a type that can create instances of <see cref="IClaimsPrincipalProvider"/>.
    /// </summary>
    public interface IClaimsPrincipalProviderFactory
    {
        /// <summary>
        /// Creates the appropriate <see cref="IClaimsPrincipalProvider"/> instance for the given <paramref name="openIddictRequest"/>.
        /// </summary>
        /// <param name="openIddictRequest">The <see cref="OpenIddictRequest"/> object to use to decide on the appropriate <see cref="IClaimsPrincipalProvider"/> implementation.</param>
        /// <returns>An implementation of <see cref="IClaimsPrincipalProvider"/>.</returns>
        IClaimsPrincipalProvider CreateProvider(OpenIddictRequest openIddictRequest);
    }
}