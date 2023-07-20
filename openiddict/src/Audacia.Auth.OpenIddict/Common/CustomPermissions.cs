namespace Audacia.Auth.OpenIddict.Common;

/// <summary>
/// Helper class to allow custom permissions to be easily created.
/// </summary>
public static class CustomPermissions
{
    /// <summary>
    /// Creates a grant type custom permission for the given <paramref name="grantTypeValue"/>.
    /// </summary>
    /// <param name="grantTypeValue">The grant type for which to create the permission.</param>
    /// <returns>A formatted string that can be used as an OpenIddict permission.</returns>
    public static string GrantType(string grantTypeValue) => $"gt:{grantTypeValue}";
}
