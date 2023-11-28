using System;
using System.Security.Cryptography.X509Certificates;
using Audacia.Auth.OpenIddict.DependencyInjection;

namespace Audacia.Auth.OpenIddict.Common.Extensions;

/// <summary>
/// Extensions to the <see cref="string"/> type.
/// </summary>
public static class StringExtensions
{
    private const string ObfuscatedString = "********";

    /// <summary>
    /// Obfuscates the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to obfuscate.</param>
    /// <returns>An obfuscated <see cref="string"/>.</returns>
    internal static string Obfuscate(this string value) => ObfuscatedString;

    /// <summary>
    /// Parses the given <see cref="string"/> into a <see cref="StoreLocation"/> value.
    /// </summary>
    /// <param name="rawValue">The <see cref="string"/> to parse.</param>
    /// <returns>A <see cref="StoreLocation"/> item that matches the given <paramref name="rawValue"/>.</returns>
    /// <exception cref="OpenIddictConfigurationException">If the given <see cref="string"/> does not match a <see cref="StoreLocation"/> item.</exception>
    public static StoreLocation ParseStoreLocation(this string? rawValue)
    {
        StoreLocation storeLocation;
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            storeLocation = StoreLocation.CurrentUser;
        }
        else if (!Enum.TryParse(rawValue, out storeLocation))
        {
            throw new OpenIddictConfigurationException($"'{rawValue}' is not a valid certificate store location. Acceptable values are 'LocalMachine' and 'CurrentUser'.");
        }

        return storeLocation;
    }
}
