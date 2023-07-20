using System;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Seeding.EntityFrameworkSupport;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFramework;

internal class Program
{
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
            var seeder = new EntityFrameworkSeeder<int>(
                parsedArguments.IdentityProjectBasePath,
                parsedArguments.IdentityProjectName,
                parsedArguments.DatabaseConnectionStringName);
            return seeder.SeedAsync();
        }

        if (parsedArguments.OpenIddictEntitiesKeyType == typeof(string))
        {
            var seeder = new EntityFrameworkSeeder<string>(
                parsedArguments.IdentityProjectBasePath,
                parsedArguments.IdentityProjectName,
                parsedArguments.DatabaseConnectionStringName);
            return seeder.SeedAsync();
        }

        if (parsedArguments.OpenIddictEntitiesKeyType == typeof(Guid))
        {
            var seeder = new EntityFrameworkSeeder<Guid>(
                parsedArguments.IdentityProjectBasePath,
                parsedArguments.IdentityProjectName,
                parsedArguments.DatabaseConnectionStringName);
            return seeder.SeedAsync();
        }

        throw new NotSupportedException($"Key type {parsedArguments.OpenIddictEntitiesKeyType} is not supported.");
    }
}
