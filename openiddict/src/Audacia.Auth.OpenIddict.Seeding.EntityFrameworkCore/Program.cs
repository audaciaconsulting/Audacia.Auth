using System;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Seeding.EntityFrameworkSupport;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFrameworkCore
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // Exec command: Start-Process -NoNewWindow -FilePath .\Audacia.Auth.OpenIddict.Seeding.EntityFrameworkCore.exe -ArgumentList "C:\Repos\Audacia.Templates\Audacia.Template\src\Audacia.Template.Identity", "OpenIdConnectConfig", "int", "DatabaseContext"
            /*
             * Expected args:
             *  [0] OpenID Connect config section name
             *  [1] OpenIddict entities key type: should be "int", "string" or "Guid"
             *  [2] Database connection string name
             */

            var parsedArguments = new ParsedEntityFrameworkArguments(args);
            await SeedAsync(parsedArguments).ConfigureAwait(false);
        }

        private static Task SeedAsync(ParsedEntityFrameworkArguments parsedArguments)
        {
            if (parsedArguments.OpenIddictEntitiesKeyType == typeof(int))
            {
                var seeder = new EntityFrameworkCoreSeeder<int>(
                    parsedArguments.OpenIdConnectConfigSectionName,
                    parsedArguments.DatabaseConnectionStringName);
                return seeder.SeedAsync();
            }

            if (parsedArguments.OpenIddictEntitiesKeyType == typeof(string))
            {
                var seeder = new EntityFrameworkCoreSeeder<string>(
                    parsedArguments.OpenIdConnectConfigSectionName,
                    parsedArguments.DatabaseConnectionStringName);
                return seeder.SeedAsync();
            }

            if (parsedArguments.OpenIddictEntitiesKeyType == typeof(Guid))
            {
                var seeder = new EntityFrameworkCoreSeeder<Guid>(
                    parsedArguments.OpenIdConnectConfigSectionName,
                    parsedArguments.DatabaseConnectionStringName);
                return seeder.SeedAsync();
            }

            throw new NotSupportedException($"Key type {parsedArguments.OpenIddictEntitiesKeyType} is not supported.");
        }
    }
}
