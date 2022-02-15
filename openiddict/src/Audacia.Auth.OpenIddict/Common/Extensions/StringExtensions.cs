namespace Audacia.Auth.OpenIddict.Common.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="string"/> type.
    /// </summary>
    internal static class StringExtensions
    {
        private const string ObfuscatedString = "********";

        /// <summary>
        /// Obfuscates the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to obfuscate.</param>
        /// <returns>An obfuscated <see cref="string"/>.</returns>
        internal static string Obfuscate(this string value) => ObfuscatedString;
    }
}
