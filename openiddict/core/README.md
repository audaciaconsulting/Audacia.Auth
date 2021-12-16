# Introduction

The `Audacia.Auth.OpenIddict` library is designed to make it easier for apps to use OpenIddict as an OpenID Connect and OAuth provider. It uses ASP.NET Core Identity for the underlying user login and provides the following functionality:
- Implements the '/connect/authorize', '/connect/token' and '/connect/userinfo' endpoints
- Handles the issuing of access tokens and id tokens using the following flows:
    - Authorization Code + PKCE (for UI clients)
    - Client Credentials (for server-side/API clients)
    - Resource Owner Password Credentials (for test automation clients)
- Provides a means for additional claims to be added to tokens via an implementation of `IAdditionalClaimsProvider`

# How to Use

There are two use cases for this library:
- Adding authentication to a new project
- Replacing IdentityServer4 with OpenIddict in an existing project

If this is being added to a new project, first of all you'll need to create a new ASP.NET Core MVC project with the appropriate models, views and controllers to handle login and logout (plus forgot password and change password if required).

First of all install the `Audacia.Auth.OpenIddict` NuGet package. If you are adding 

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

For example, in Entity Framework Core the registration could look something like this:
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
    base.OnModelCreating(builder);
    builder.UseOpenIddict<int>(); // ADD THIS
    builder.ApplyConfigurations();
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

# Replacing IdentityServer4 Checklist

If you are replacing IdentityServer4 with OpenIddict, here is a high-level checklist of what needs to be done. More details for each point can be found in the relevant section above:
- [ ] Uninstall IdentityServer4 packages
- [ ] Install the following NuGet packages in your 'identity' project (whichever project acts as the authorization server):
   - `Audacia.Auth.OpenIddict`
   - `Audacia.Auth.OpenIddict.QuartzCleanup` (removes old authorization data)
   - `Audacia.Auth.OpenIddict.Seeding` (seeds configuration data into the database)
   - `OpenIddict.EntityFrameworkCore` or `OpenIddict.EntityFramework`, as necessary
- [ ] Install the `OpenIddict.EntityFrameworkCore` (or `OpenIddict.EntityFramework`) package in your Entity Framework project
- [ ] If you have a separate API project, install the `OpenIddict.AspNetCore` package
- [ ] Add Entity Framework (Core) setup (see above)
- [ ] Add OpenIddict services (see above)
- [ ] Write code to convert existing IdentityServer configuration to an `OpenIdConnectConfig` object (see above)
   - This must include adding scopes to the configuration so that they are registered in the database; this is an `OpenIdConnectScope` object that has 
- [ ] Set some claim types in ASP.NET Core Identity setup (see above)
- [ ] If a custom profile service has been written, modify it to implement the `IProfileService<TUser, TKey>` interface (see above)
- [ ] Remove any IdentityServer4-specific code, such as:
   - `ApiResources`
   - `IdentityResources`
   - `Clients`
   - `PersistedGrantStore`
   - Uses of `IIdentityServerInteractionService`