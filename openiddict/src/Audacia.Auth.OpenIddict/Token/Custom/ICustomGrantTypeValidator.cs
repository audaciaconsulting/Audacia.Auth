using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace Audacia.Auth.OpenIddict.Token.Custom;

/// <summary>
/// Represents a type that can validate a <see cref="OpenIddictRequest"/> for a custom grant type.
/// </summary>
/// <typeparam name="TUser">The type of user.</typeparam>
public interface ICustomGrantTypeValidator<TUser> where TUser : class
{
    /// <summary>
    /// Gets the grant type supported by this provider.
    /// </summary>
    string GrantType { get; }

    /// <summary>
    /// Asynchronously gets a <see cref="CustomGrantTypeValidationResponse{TUser}"/> object from the given <paramref name="openIddictRequest"/>.
    /// If a response cannot be created then an <see cref="InvalidGrantException"/> should be thrown.
    /// </summary>
    /// <param name="openIddictRequest">The <see cref="OpenIddictRequest"/> to validate.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when completed, contains a <see cref="CustomGrantTypeValidationResponse{TUser}"/> object.</returns>
    Task<CustomGrantTypeValidationResponse<TUser>> ValidateAsync(OpenIddictRequest openIddictRequest);
}
