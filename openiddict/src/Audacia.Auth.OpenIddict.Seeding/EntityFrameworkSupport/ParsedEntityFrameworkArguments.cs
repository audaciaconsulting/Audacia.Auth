using System;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFrameworkSupport
{
    /// <summary>
    /// Represents the parsed command line arguments for Entity Framework 6.x and Entity Framework Core.
    /// </summary>
    public class ParsedEntityFrameworkArguments
    {
        /// <summary>
        /// Gets the name of the configuration section that maps to an <see cref="Common.Configuration.OpenIdConnectConfig"/> object.
        /// </summary>
        public string OpenIdConnectConfigSectionName { get; }

        /// <summary>
        /// Gets the type of the key of OpenIddict entities.
        /// </summary>
        public Type OpenIddictEntitiesKeyType { get; }

        /// <summary>
        /// Gets the name of the database connection string in the applications settings.
        /// </summary>
        public string DatabaseConnectionStringName { get; }

        /// <summary>
        /// Initializes an instance of <see cref="ParsedEntityFrameworkArguments"/>.
        /// </summary>
        /// <param name="rawArguments">The raw arguments that were passed via the command line.</param>
        /// <exception cref="ArgumentNullException">When the given <paramref name="rawArguments"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">When the given <paramref name="rawArguments"/> does not contain exactly four items.</exception>
        public ParsedEntityFrameworkArguments(string[] rawArguments)
        {
            if (rawArguments == null)
            {
                throw new ArgumentNullException(nameof(rawArguments));
            }

            if (rawArguments.Length != 3)
            {
                throw new ArgumentException("Exactly three arguments are expected.");
            }

            OpenIdConnectConfigSectionName = GetOpenIdConnectConfigSectionName(rawArguments);
            OpenIddictEntitiesKeyType = GetOpenIddictEntitiesKeyType(rawArguments);
            DatabaseConnectionStringName = GetDatabaseConnectionStringName(rawArguments);
        }

        private static string GetOpenIdConnectConfigSectionName(string[] rawArguments) =>
            rawArguments[0];

        private static Type GetOpenIddictEntitiesKeyType(string[] rawArguments) =>
            rawArguments[1] switch
            {
                "int" => typeof(int),
                "string" => typeof(string),
                "Guid" => typeof(Guid),
                _ => throw new NotSupportedException($"Key type {rawArguments[2]} is not supported.")
            };

        private static string GetDatabaseConnectionStringName(string[] rawArguments) =>
            rawArguments[2];
    }
}
