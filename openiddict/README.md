# Introduction

The `Audacia.Auth.OpenIddict` library is designed to make it easier for apps to use OpenIddict as an OpenID Connect and OAuth provider. It uses ASP.NET Core Identity for the underlying user login and provides the following functionality:
- Implements the '/connect/authorize', '/connect/token' and '/connect/userinfo' endpoints
- Handles the issuing of access tokens and id tokens using the following flows:
    - Authorization Code + PKCE (for UI clients)
    - Client Credentials (for server-side/API clients)
    - Resource Owner Password Credentials (for test automation clients)
- Provides a means for additional claims to be added to tokens via an implementation of `IAdditionalClaimsProvider`

There are two use cases for this library:
- Adding authentication to a new project
- Replacing IdentityServer4 with OpenIddict in an existing project

# How to Use

If this is being added to a new project, first of all you'll need to create a new ASP.NET Core MVC project with the appropriate models, views and controllers to handle login and logout (plus forgot password and change password if required).

If you are replacing IdentityServer4 with OpenIddict, you'll need to uninstall all IdentityServer4 NuGet packages and remove any references to IdentityServer4 code, such as:
- `ApiResources`
- `IdentityResources`
- `Clients`
- `PersistedGrantStore`
- Uses of `IIdentityServerInteractionService`

For both new projects and IdentityServer4 replacement, here is a high-level checklist of what needs to be done. More details for each point can be found in the relevant section below:
- [ ] Install the following NuGet packages in your 'identity' project (whichever project acts as the authorization server):
   - `Audacia.Auth.OpenIddict`
   - `Audacia.Auth.OpenIddict.QuartzCleanup` (removes old authorization data)
   - For Entity Framework Core projects:
      - `Audacia.Auth.OpenIddict.Seeding.EntityFrameworkCore` (seeds configuration data into the database)
      - `OpenIddict.EntityFrameworkCore`
   - For Entity Framework 6.x projects:
      - `Audacia.Auth.OpenIddict.Seeding.EntityFramework` (seeds configuration data into the database)
      - `Audacia.Auth.OpenIddict.EntityFramework` (this is a thin wrapper around `OpenIddict.EntityFramework` that makes it easier to work with `int` or `Guid` primary keys for OpenIddict entities)
- [ ] Install the `OpenIddict.EntityFrameworkCore` or `Audacia.Auth.OpenIddict.EntityFramework` package in your Entity Framework project
- [ ] If you have a separate API project, install the `OpenIddict.AspNetCore` package there
- [ ] Add Entity Framework (Core) setup (see [here](#entity-framework-and-entity-framework-core))
- [ ] Add OpenIddict services (see [here](#register-openiddict-services))
- [ ] Register the OpenIddict controllers (see [here](#configure-mvc-controllers))
- [ ] Change API authentication to use OpenIddict (see [here](#api-authentication))
- [ ] If replacing IdentityServer4, write code to convert existing IdentityServer configuration to an `OpenIdConnectConfig` object (see [here](#configuration-in-appsettingsjson))
   - This must include adding scopes to the configuration so that they are registered in the database; this is an `OpenIdConnectScope` object that has 
- [ ] Set some claim types in ASP.NET Core Identity setup (see [here](#aspnet-core-identity-configuration))
- [ ] If a custom profile service and/or additional claims provider are required, implement using the information [here](#adding-additional-claims-to-tokens)

## Configuration in appsettings.json

The `Audacia.Auth.OpenIddict` library uses a configuration object of type `OpenIdConnectConfig` (defined in the `Audacia.Auth.OpenIddict.Common.Configuration` namespace). If you are adding authentication to a new project, the easiest way to provide this object is by defining it in the appsettings.json file (example below). However if you are replacing IdentityServer4 in an existing project then it is generally simpler to leave the configuration file as-is and to convert from the existing structure to `OpenIdConnectConfig` in code.

```json
{
    "OpenIdConnectConfig": {
        "EncryptionCertificateThumbprint": "TBC",
        "SigningCertificateThumbprint": "TBC",
        "Url": "https://localhost:44374",
        "ApiClients": [
            {
                "ClientId": "ApiClient",
                "ClientSecret": "xxx",
                "ClientScopes": [ "api" ]
            }
        ],
        "UiClients": [
            {
                "ClientScopes": [ "api" ],
                "ClientId": "AngularClient",
                "BaseUrl": "https://localhost:44351",
                "RedirectUris": [
                    "https://localhost:44351",
                    "https://localhost:44351/auth-callback",
                    "https://localhost:44351/assets/silent-renew.html"
                ],
                "AccessTokenLifetime": {
                    "Value": "30",
                    "Type": "Minutes"
                }
            },
            {
                "ClientId": "SwaggerClient",
                "BaseUrl": "https://localhost:44397",
                "ClientScopes": [ "api" ],
                "RedirectUris": [
                    "https://localhost:44397",
                    "https://localhost:44397/swagger/oauth2-redirect.html"
                ]
            }
        ],
        "TestAutomationClients": [
            {
                "ClientId": "TestAutomationClient",
                "ClientScopes": [ "api" ],
                "ClientSecret": "xxx"
            }       
        ],
        "Scopes": [
            {
                "Name": "api",
                "Resources": [ "ApiClient" ]
            }
        ]
    }
}
```

## Register OpenIddict Services

You must register the necessary OpenIddict services with the ASP.NET Core dependency injection system. Because a lot of the types defined by `Audacia.Auth.OpenIddict` are generic on the User type and the User's primary key type (this is because both `UserManager` and `SignInManager` from ASP.NET Core Identity are generic), when registering the services you must provide the necessary generic parameters.

This can be achieved by calling an extension method on `IServiceCollection` provided by `Audacia.Auth.OpenIddict`. As well as taking the generic parameters, this method also takes a delegate which can be used to further configure `OpenIddict`. This delegate is of type `Action<OpenIddictCoreBuilder>`, so core configuration can be provided, such as the ORM provider to use for data persistence.

OpenIddict saves issued tokens to the database, so to avoid that data building up over time, it is important to remove old data periodically. OpenIddict comes with built-in support for doing this cleanup in a background job using Quartz. This is implemented in the `Audacia.Auth.OpenIddict.QuartzCleanup` NuGet package. Unless you have another mechanism for cleaning up this data (such as a scheduled Azure Function) then you should use Quartz cleanup.

For example, suppose your user type is `ApplicationUser` and the primary key of `ApplicationUser` is an `int`, and you are using `EntityFrameworkCore` as your ORM. If you are also using the built-in Quartz cleanup, registering the services would look something like this (without the Quartz cleanup the code would be identical, it would just call the `AddOpenIddict` method rather than `AddOpenIddictWithCleanup`):
```csharp
services.AddOpenIddictWithCleanup<ApplicationUser, int>(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<MyDbContext>()
            .ReplaceDefaultEntities<int>();
    },
    openIdConnectConfig,
    hostingEnvironment);
```

The additional parameters are:
- `openIdConnectConfig` is an instance of `OpenIdConnectConfig` (see appsettings.json section above)
- `hostingEnvironment` is an instance of `IWebHostEnvironment`

## Entity Framework and Entity Framework Core

OpenIddict must be registered with both Entity Framework and Entity Framework Core. The relevant methods are from OpenIddict libraries: `OpenIddict.EntityFrameworkCore` and `OpenIddict.EntityFramework`.

### Entity Framework Core

In Entity Framework Core the registration could look something like this:
```csharp
services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer()
        .UseOpenIddict<int>();
});
```

You will also need to run a database migration to add the necessary OpenIddict tables to the database, and this may require some additional code to tell the EF model about OpenIddict. For example in Entity Framework Core you may need something like this:
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    // Other configurations here....
    builder.UseOpenIddict<int>(); // ADD THIS
    base.OnModelCreating(builder);
}
```

### Entity Framework 6.x

In Entity Framework 6.x you just need to call the appropriate method when creating the model. The `Audacia.Auth.OpenIddict.EntityFramework` package has the same extension method that OpenIddict provides out of the box for EF Core, so your `OnModelCreating` method will look something like this:
```csharp
protected override void OnModelCreating(DbModelBuilder builder)
{
    // Other configurations here....
    builder.UseOpenIddict<int>(); // ADD THIS
    base.OnModelCreating(builder);
}
```

## ASP.NET Core Identity Configuration

Some claim types must be set in the ASP.NET Core Identity configuration as follows:
```csharp
services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Note the claim types should be constants somewhere, or use the IdentityModel NuGet package
    // with its JwtClaimTypes class
    options.ClaimsIdentity.UserIdClaimType = "sub";
    options.ClaimsIdentity.UserNameClaimType = "email";
    options.ClaimsIdentity.RoleClaimType = "role";
    // More code here....
});
```

## Configure MVC Controllers

The controllers that handle the OpenID Connect endpoints are generic (on the User type and the User's primary key type), which means they must be specifically registered with ASP.NET Core in order to be discovered.

This can be achieved by calling an extension method on `IMvcBuilder`, providing the necessary generic parameters. This will usually be as a method call chained on the end of a call to `AddControllersWithViews()`. For example, suppose your user type is `ApplicationUser` and the primary key of `ApplicationUser` is an `int`:
```csharp
services.AddControllersWithViews()
    .ConfigureOpenIddict<ApplicationUser, int>();
```

## Adding Additional Claims to Tokens

If you require any custom claims beyond the standard ones issued there are two main mechanisms by which this can be achieved.

### Additional Claims Provider

The `IAdditionalClaimsProvider<TUser, TKey>` interface is designed to provide claims that should be added to tokens based on the information present in the authenticated user (i.e. in the `TUser` instance). This can be done by either:
- Deriving from the `DefaultAdditionalClaimsProvider<TUser, TKey>` base class and overriding the `CustomClaimFactories` property
- Implementing the `IAdditionalClaimsProvider<TUser, TKey>` interface (if you don't want the claims that `DefaultAdditionalClaimsProvider<TUser, TKey>` adds, which is currently just `email`)

### Profile Service

The `Audacia.Auth.OpenIddict` library also provides an `IProfileService<TUser, TKey>` interface, which performs essentially the same role as the `IProfileService` interface that is part of IdentityServer4. If you need to perform logic or make calls to a database or external API, the `GetClaimsAsync` method of `IProfileService<TUser, TKey>` is the appropriate place for this.

The provided implementation of `IProfileService<TUser, TKey>`, `DefaultProfileService<TUser, TKey>`, adds the claims from `IAdditionalClaimsProvider<TUser, TKey>` so you can derive from this base class rather than implement the interface directly.

An example implementation might be:
```csharp
public class CustomProfileService : DefaultProfileService<ApplicationUser, int>
{
    public override async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user, ClaimsPrincipal claimsPrincipal)
    {
        var claims = new List<Claim>();
        claims.AddRange(await base.GetClaimsAsync(user, claimsPrincipal));
        await AddCustomClaimsAsync(user, claimsPrincipal, claims);

        return claims;
    }

    private async Task AddCustomClaimsAsync(ApplicationUser user, ClaimsPrincipal claimsPrincipal, List<Claim> claims)
    {
        // Custom logic here
    }
}
```

## API Authentication

Any API must use OpenIddict to validate access tokens. This can be achieved using the code below (where `services` is an instance of `IServiceCollection`). If you are replacing `IdentityServer4` then this will likely replace a call to `AddIdentityServerAuthentication`.

```csharp
services
    .AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(/*Identity app url*/);

        options.AddAudiences(/*Client ID of the API*/);

        // DON'T INCLUDE THIS SECTION IF OPENIDDICT IS HOSTED IN THE SAME WEB APP AS THE API
        options.UseIntrospection()
            .SetClientId(/*Client ID of the API*/)
            .SetClientSecret(/*Client secret of the API*/);

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });
```

## Seeding Clients and Scopes in the Database

OpenIddict requires a database in which to store clients (which it calls 'applications') and scopes. Generally speaking client and scope data comes from configuration, therefore there needs to be a mechanism to get the data from configuration into the database in each environment.

When running the app locally, a hosted service can be used to do this on app startup. The necessary worker service is provided in the `Audacia.Auth.OpenIddict.Seeding` package, and should be called as follows (where `services` is an instance of `IServiceCollection` and `environment` is an instance of `IWebHostEnvironment`):
```csharp
if (environment.IsDevelopment())
{
    services.AddLocalSeeding();
}
```

In a deployed environment the seeding should be done as part of the deployment pipeline. There are steps in the `Audacia.Build` repo to perform the seeding via a custom .NET tool. Provided you are using the standard OpenIddict entities and managers with either EF Core or EF 6, you can use the steps in either `openiddict-seeding-efcore.yaml` or `openiddict-seeding-ef6.yaml`. With the template functionality available in Azure Pipelines YAML, for EF Core the YAML could look something like this:
```yaml
steps:
  - template: /steps/deploy/openiddict-seeding-efcore.yaml@templates
    parameters:
      identityAppName: 'MyApp.Identity'
      openIdConnectConfigSectionName: 'OpenIdConnectConfig'
      openIddictEntitiesKeyType: 'int'
      databaseConnectionStringName: 'MyDatabaseContext'
```

### Mapping Config

**IMPORTANT!!** You must implement the `IOpenIdConnectConfigMapper` interface in your Identity project so that the seeding tool can create an instance of `OpenIdConnectConfig`. The implementing class must have a parameterless constructor (this is so the seeding tool can create an instance of it).

If your appsettings already contains the config in the correct structure then this implementation can be:
```csharp
public class OpenIdConnectConfigMapper : IOpenIdConnectConfigMapper
{
    public OpenIdConnectConfig Map(IConfiguration configuration)
    {
        return configuration.GetSection("ConfigKey").Get<OpenIdConnectConfig>();
    }
}
```

However if your appsettings contains the config in a different structure then some custom mapping code will be needed.