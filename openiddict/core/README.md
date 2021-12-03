# Introduction

The `Audacia.Auth.OpenIddict` library is designed to make it easier for apps to use OpenIddict as an OpenID Connect and OAuth provider. It uses ASP.NET Core Identity for the underlying user login and provides the following functionality:
- Implements the '/connect/authorize', '/connect/token' and '/connect/userinfo' endpoints
- Handles the issuing of access tokens and id tokens using the following flows:
    - Authorization Code + PKCE (for UI clients)
    - Client Credentials (for server-side/API clients)
    - Resource Owner Password Credentials (for test automation clients)
- Provides a means for additional claims to be added to tokens via an implementation of `IAdditionalClaimsProvider`

# How to Use

After installing the `Audacia.Auth.OpenIddict` NuGet package there are three main areas that must be modified.

## Configuration in appsettings.json

The `Audacia.Auth.OpenIddict` library uses a configuration object of type `OpenIdConnectConfig` (defined in the `Audacia.Auth.OpenIddict.Common.Configuration` namespace). The easiest way to provide this object is by defining it in the appsettings.json file. An example would be:
```json

```

## Register OpenIddict Services

You must register the necessary OpenIddict services with the ASP.NET Core dependency injection system. Because a lot of the types defined by `Audacia.Auth.OpenIddict` are generic on the User type and the User's primary key type (this is because both `UserManager` and `SignInManager` from ASP.NET Core Identity are generic), when registering the services you must provide the necessary generic parameters.

This can be achieved by calling an extension method on `IServiceCollection` provided by `Audacia.Auth.OpenIddict`. As well as taking the generic parameters, this method also takes a delegate which can be used to further configure `OpenIddict`. This delegate is of type `Action<OpenIddictCoreBuilder>`, so core configuration can be provided, such as the ORM provider to use for data persistence. 

For example, suppose your user type is `ApplicationUser` and the primary key of `ApplicationUser` is an `int`, and you are using `EntityFrameworkCore` as your ORM. Registering the services would look something like this:
```csharp
services.AddOpenIddict<ApplicationUser, int>(
    options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<MyDbContext>()
            .ReplaceDefaultEntities<int>();
    },
    openIdConnectConfig,
    hostingEnvironment);
)
```

The additional parameters are:
- `openIdConnectConfig` is an instance of `OpenIdConnectConfig` (see appsettings.json section above)
- `hostingEnvironment` is an instance of `IWebHostEnvironment`

## Entity Framework and Entity Framework Core

OpenIddict must be registered with both Entity Framework and Entity Framework Core.

For example, in Entity Framework Core the registration could look something like this:
```csharp
services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer()
        .UseOpenIddict<int>();
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

If you require any custom claims beyond the standard ones issued, you can add them by either:
- Deriving from the `DefaultAdditionalClaimsProvider<TUser, TKey>` base class and overriding the `CustomClaimFactories` property
- Implementing the `IAdditionalClaimsProvider<TUser, TKey>` interface (if you don't want the claims that `DefaultAdditionalClaimsProvider<TUser, TKey>` adds, which is currently just `email`)