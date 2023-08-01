using System;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFrameworkSupport;

/// <summary>
/// Represents the parsed command line arguments for Entity Framework 6.x and Entity Framework Core.
/// </summary>
public class ParsedEntityFrameworkArguments
{
    /// <summary>
    /// Gets the path to the directory containing the appsettings.json file.
    /// </summary>
    public string IdentityProjectBasePath { get; }

    /// <summary>
    /// Gets the name of the configuration section that maps to an <see cref="Common.Configuration.OpenIdConnectConfig"/> object.
    /// </summary>
    public string IdentityProjectName { get; }

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

        if (rawArguments.Length != 4)
        {
            throw new ArgumentException("Exactly four arguments are expected.");
        }

        IdentityProjectBasePath = GetIdentityProjectFilepath(rawArguments);
        IdentityProjectName = GetIdentityProjectName(rawArguments);
        OpenIddictEntitiesKeyType = GetOpenIddictEntitiesKeyType(rawArguments);
        DatabaseConnectionStringName = GetDatabaseConnectionStringName(rawArguments);
    }

    private static string GetIdentityProjectFilepath(string[] rawArguments) =>
        rawArguments[0];

    private static string GetIdentityProjectName(string[] rawArguments) =>
        rawArguments[1];

    private static Type GetOpenIddictEntitiesKeyType(string[] rawArguments) =>
        rawArguments[2] switch
        {
            "int" => typeof(int),
            "string" => typeof(string),
            "Guid" => typeof(Guid),
            _ => throw new NotSupportedException($"Key type {rawArguments[2]} is not supported.")
        };

    private static string GetDatabaseConnectionStringName(string[] rawArguments) =>
        rawArguments[3];
}
