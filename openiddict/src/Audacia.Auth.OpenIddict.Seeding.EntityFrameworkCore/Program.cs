using System;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Seeding.EntityFrameworkSupport;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFrameworkCore;

/// <summary>
/// Main program, run on initialization.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Main program, run on initialization.
    /// </summary>
    /// <param name="args">Any arguments provided when program is executed.</param>
    /// <returns>A task to execute program.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "ACL1005:Asynchronous method name is not suffixed with 'Async'.", Justification = "The `Main` method is the entry point of a C# application.")]
    public static async Task Main(string[] args)
    {
        /*
         * Expected args:
         *  [0] Identity project filepath
         *  [1] Identity project name
         *  [2] OpenIddict entities key type: should be "int", "string" or "Guid"
         *  [3] Database connection string name
         */

        var parsedArguments = new ParsedEntityFrameworkArguments(args);
        await SeedAsync(parsedArguments).ConfigureAwait(false);
    }

    private static Task SeedAsync(ParsedEntityFrameworkArguments parsedArguments)
    {
        if (parsedArguments.OpenIddictEntitiesKeyType == typeof(int))
        {
            var seeder = new EntityFrameworkCoreSeeder<int>(
                parsedArguments.IdentityProjectBasePath,
                parsedArguments.IdentityProjectName,
                parsedArguments.DatabaseConnectionStringName);
            return seeder.SeedAsync();
        }

        if (parsedArguments.OpenIddictEntitiesKeyType == typeof(string))
        {
            var seeder = new EntityFrameworkCoreSeeder<string>(
                parsedArguments.IdentityProjectBasePath,
                parsedArguments.IdentityProjectName,
                parsedArguments.DatabaseConnectionStringName);
            return seeder.SeedAsync();
        }

        if (parsedArguments.OpenIddictEntitiesKeyType == typeof(Guid))
        {
            var seeder = new EntityFrameworkCoreSeeder<Guid>(
                parsedArguments.IdentityProjectBasePath,
                parsedArguments.IdentityProjectName,
                parsedArguments.DatabaseConnectionStringName);
            return seeder.SeedAsync();
        }

        throw new NotSupportedException($"Key type {parsedArguments.OpenIddictEntitiesKeyType} is not supported.");
    }
}
