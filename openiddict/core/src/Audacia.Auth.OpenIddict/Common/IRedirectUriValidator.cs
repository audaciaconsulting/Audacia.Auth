using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// Represents a type that can validate the redirect uri for a client.
    /// </summary>
    public interface IRedirectUriValidator
    {
        /// <summary>
        /// Validates the redirect uri represented by the given <paramref name="address"/>.
        /// </summary>
        /// <param name="address">The redirect uri to validate.</param>
        /// <param name="client">The client that has requested the redirection to the given <paramref name="address"/>.</param>
        /// <returns><see langword="true"/> if the redirect uri is valid, otherwise <see langword="false"/>.</returns>
        Task<bool> ValidateAsync(string address, OpenIddictApplicationDescriptor client);
    }
}
