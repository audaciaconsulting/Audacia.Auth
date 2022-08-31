[[_TOC_]]

# Introduction

The `Audacia.Auth.OpenIddict` library is designed to make it easier for apps to use OpenIddict as an OpenID Connect and OAuth provider. It uses ASP.NET Core Identity for the underlying user login and provides the following functionality:
- Implements the `/connect/authorize`, `/connect/token` and `/connect/userinfo` endpoints.
- Handles the issuing of access tokens and id tokens using the following flows:
    - Authorization Code + PKCE (for UI clients).
    - Client Credentials (for server-side/API clients).
    - Resource Owner Password Credentials (for test automation clients).
- Provides a means for additional claims to be added to tokens via an implementation of `IAdditionalClaimsProvider`.

There are two use cases for this library:
- Adding authentication to a new project.
- Replacing IdentityServer4 with OpenIddict in an existing project.

# How to Use

## Pre-requisites

There are certain pre-requisites that should be done before using this guide. What exactly needs to be done depends on whether you are adding this library to a new project, or replacing IdentityServer on an existing project.

### Adding to a New Project

If this is being added to a new project, first of all you'll need to create a new ASP.NET Core MVC project with the appropriate models, views and controllers to handle login and logout (plus forgot password and change password if required).

Steps 1-7 in the [Audacia.Training OpenIddict process](https://dev.azure.com/audacia/Audacia.Training/_git/Audacia.Training.Auth?path=/docs/openiddict-auth-process.md&version=GBmain&_a=preview&anchor=identity-app) can be followed to implement the login part of this (including seeding a user in the database). You should ensure that you can perform a successful login and be issued with an authentication cookie once this has been completed.

### Replacing IdentityServer

If you are replacing IdentityServer4 with OpenIddict, you'll need to uninstall all IdentityServer4 NuGet packages and remove any references to IdentityServer4 code, such as:
- `ApiResources`.
- `IdentityResources`.
- `Clients`.
- `PersistedGrantStore`.
- Uses of `IIdentityServerInteractionService`.

## Identity App and EF Changes

The biggest changes are needed in the 'Identity' app (this is the application that handles the login process using ASP.NET Core Identity, which is usually the app that has the login page). This may be a standalone project, or it may be hosted in the same project as the API. Also included in this section are changes related to Entity Framework.

On completion of this section you should test that you can:
- Login and logout successfully.
- Obtain an access token via the 'client credentials' and 'resource owner password credentials' flows.

### Install NuGet Packages

Install the following NuGet packages in your Identity project:
- `Audacia.Auth.OpenIddict`.
- `Audacia.Auth.OpenIddict.QuartzCleanup` (removes old authorization data).
- `Audacia.Auth.OpenIddict.Seeding` (seeds configuration data in the database when running locally).
- For Entity Framework Core, `OpenIddict.EntityFrameworkCore`.
- For Entity Framework 6.x, `Audacia.Auth.OpenIddict.EntityFramework` (this is a thin wrapper around `OpenIddict.EntityFramework` that makes it easier to work with `int` or `Guid` primary keys for OpenIddict entities).

Install one of the following NuGet packages in your Data/Entity Framework project:
- For Entity Framework Core, `OpenIddict.EntityFrameworkCore`.
- For Entity Framework 6.x, `Audacia.Auth.OpenIddict.EntityFramework`.

### Configuration in appsettings.json

The `Audacia.Auth.OpenIddict` library uses a configuration object of type `OpenIdConnectConfig` (defined in the `Audacia.Auth.OpenIddict.Common.Configuration` namespace).

If you are adding authentication to a new project, the easiest way to provide this object is by defining it in the appsettings.json file (example below).

However if you are replacing IdentityServer4 in an existing project then it is generally simpler to leave the configuration file as-is and to convert from the existing structure to `OpenIdConnectConfig` in code. If you are seeding configuration data in your deployment pipeline, as described [here](#seeding-the-database-in-a-pipeline), and are converting the config from an existing structure, then you will need to provide an implementation of `IOpenIdConnectConfigMapper` which should contain the conversion code. However as long as your configuration section is named `"OpenIdConnectConfig"` and can be mapped directly to an `OpenIdConnectConfig` object then you don't need to provide an `IOpenIdConnectConfigMapper` implementation, and the seeding process with automatically pull the configuration in.

The structure of the config is as follows:
- `EncryptionCertificateThumbprint`: The thumbprint of the self-signed certificate that will be used to encrypt tokens; note this is not required when developing locally, so no certificates need to be generated for local development and this setting can be left blank; when set for deployed environments it must be a different certificate to the one used for signing (see [here](#token-signing-and-encryption-certificates) for more information).
- `SigningCertificateThumbprint`: The thumbprint of the self-signed certificate that will be used to sign tokens; note this is not required when developing locally, so no certificates need to be generated for local development and this setting can be left blank; when set for deployed environments it must be a different certificate to the one used for encryption (see [here](#token-signing-and-encryption-certificates) for more information).
- `CertificateStoreLocation`: Optional, defaults to `"CurrentUser"`; the other valid value is `"LocalMachine"`:
    - Generally speaking you should use `"CurrentUser"` (or omit entirely) if deploying to Azure App Services, and use `"LocalMachine"` if deploying to IIS.
- `Url`: The base url of the Identity app.
- `ClientCredentialsClients`: Clients that will use the Client Credentials flow; this is typically APIs and other back-end applications that don't need to obtain tokens on behalf of a user.
- `AuthorizationCodeClients`: Clients that will use the Authorization Code (with PKCE) flow; this will generally be any UI-based application (e.g. web app, mobile app).
- `ResourceOwnerPasswordClients`: Clients that will use the Resource Owner Password Credentials flow; this should be restricted to test automation clients (e.g. API testing or security testing).
- `Scopes`: Scopes define the resources (usually APIs) that can be authenticated with tokens issued by the OpenIddict service:
    - Each scope has a `Name` and a collection of `Resources`.
    - The `Name` is just a unique identifier for the scope.
    - The `Resources` are a set of clients that the scope grants access to; the values themselves should be the `ClientId` of the client to be accessed.

Within each client configuration object, the following properties can be set:
- `ClientId`: A unique identifier for the client.
- `ClientSecret`: This is effectively the password for the client; it is only required for Client Credentials and Resource Owner Password Credentials clients.
- `ClientScopes`: The scopes to which the client has access; each scope should match the `Name` of an item in the `Scopes` collection.
- `AccessTokenLifetime`: Optionally sets a custom access token lifetime.
- `BaseUrl`: The base url of the client app _(applies to Authorization Code Clients only)_.
- `RedirectUris`: The set of urls within the client app to which OpenIddict is allowed to redirect _(applies to Authorization Code Clients only)_.

An example config section is as follow; **note you should replace values as appropriate, particularly urls and client ids/secrets**:
```json
{
    "OpenIdConnectConfig": {
        "EncryptionCertificateThumbprint": "TBC",
        "SigningCertificateThumbprint": "TBC",
        "CertificateStoreLocation": "",
        "Url": "https://localhost:44374",
        "ClientCredentialsClients": [
            {
                "ClientId": "ApiClient",
                "ClientSecret": "xxx",
                "ClientScopes": [ "api" ]
            }
        ],
        "AuthorizationCodeClients": [
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
        "ResourceOwnerPasswordClients": [
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

### Entity Framework and Entity Framework Core

OpenIddict must be registered with both Entity Framework and Entity Framework Core. The relevant methods are from OpenIddict libraries: `OpenIddict.EntityFrameworkCore` and `OpenIddict.EntityFramework`.

**It is _strongly_ recommended that you create a separate database context for the OpenIddict entities.**

#### Entity Framework Core

In Entity Framework Core the registration could look something like this (where `services` is an instance of `IServiceCollection`):
```csharp
services.AddDbContext<DatabaseContext>(options =>
{
    options
        .UseSqlServer()
        .UseOpenIddict<int>();
});
```

You will also need to run a database migration to add the necessary OpenIddict tables to the database, and this may require some additional code to tell the EF model about OpenIddict. For example in Entity Framework Core you will need something like this:
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    // Other configurations here....
    builder.UseOpenIddict<int>(); // ADD THIS
    base.OnModelCreating(builder);
}
```

#### Entity Framework 6.x

In Entity Framework 6.x you just need to call the appropriate method when creating the model. The `Audacia.Auth.OpenIddict.EntityFramework` package has the same extension method that OpenIddict provides out of the box for EF Core, so your `OnModelCreating` method will look something like this:
```csharp
protected override void OnModelCreating(DbModelBuilder builder)
{
    // Other configurations here....
    builder.UseOpenIddict<int>(); // ADD THIS
    base.OnModelCreating(builder);
}
```

Note you will also need the using statement: `using Audacia.Auth.OpenIddict.EntityFramework`.

### Register OpenIddict Services

You must register the necessary OpenIddict services with the ASP.NET Core dependency injection system. Because a lot of the types defined by `Audacia.Auth.OpenIddict` are generic on the User type and the User's primary key type (because both `UserManager` and `SignInManager` from ASP.NET Core Identity are generic), when registering the services you must provide the necessary generic arguments.

This can be achieved by calling an extension method on `IServiceCollection` provided by `Audacia.Auth.OpenIddict`. As well as taking the generic parameters, this method also takes a delegate which can be used to further configure `OpenIddict`. This delegate is of type `Action<OpenIddictCoreBuilder>`, so core configuration can be provided, such as the ORM provider to use for data persistence. Alongside registering all necessary OpenIddict services, this method also adds token signing and encryption credentials (using developer credentials locally and certificates in a deployed environment) therefore this does not need to be done in application code.

OpenIddict saves issued tokens to the database, so to avoid that data building up over time, it is important to remove old data periodically. OpenIddict comes with built-in support for doing this cleanup in a background job using Quartz. This is implemented in the `Audacia.Auth.OpenIddict.QuartzCleanup` NuGet package. Unless you have another mechanism for cleaning up this data (such as a scheduled Azure Function) then you should use Quartz cleanup.

For example, suppose your user type is `ApplicationUser` and the primary key of `ApplicationUser` is an `int`, and you are using `EntityFrameworkCore` as your ORM with a database context `OpenIddictDbContext`. If you are also using the built-in Quartz cleanup, registering the services would look something like this (without the Quartz cleanup the code would be identical, it would just call the `AddOpenIddict` method rather than `AddOpenIddictWithCleanup`):
```csharp
var openIddictBuilder = services.AddOpenIddictWithCleanup<ApplicationUser, int>(options =>
    {
        options
            .UseEntityFrameworkCore()
            .UseDbContext<OpenIddictDbContext>()
            .ReplaceDefaultEntities<int>();
    },
    user => user.Id,
    openIdConnectConfig,
    hostingEnvironment);
```

The additional parameters are:
- The lambda expression `user => user.Id` is the `userIdGetter`, and just needs to be a delegate that returns the user Id.
- `openIdConnectConfig` is an instance of `OpenIdConnectConfig` (see [appsettings.json section above](#configuration-in-appsettings.json)).
- `hostingEnvironment` is an instance of `IWebHostEnvironment`.

This should replace `services.AddIdentityServer()` if you are replacing Identity Server.

If using EF6, the call to `ReplaceDefaultEntities()` should not have a type parameter. Instead, include `using Audacia.Auth.OpenIddict.EntityFramework.IntKey;` or `using Audacia.Auth.OpenIddict.EntityFramework.GuidKey;` to control the type of the primary key. Without either of these `using` statements, a `string` primary key is assumed.

**IMPORTANT:** If you need to inspect the access token that OpenIddict issues in a client application (e.g. an Angular app) then you must disable access token encryption. This can be done by adding the following line of code after the call to `AddOpenIddict`/`AddOpenIddictWithCleanup`:
```csharp
openIddictBuilder.AddServer(options => options.DisableAccessTokenEncryption());
```

### Configure MVC Controllers

The controllers that handle the OpenID Connect endpoints are generic (on the User type and the User's primary key type), which means they must be specifically registered with ASP.NET Core in order to be discovered.

This can be achieved by calling an extension method on `IMvcBuilder`, providing the necessary generic arguments. This will usually be as a method call chained on the end of a call to `AddControllersWithViews()`. For example, suppose your user type is `ApplicationUser` and the primary key of `ApplicationUser` is an `int`:
```csharp
services
    .AddControllersWithViews()
    .ConfigureOpenIddict<ApplicationUser, int>();
```

### Seeding Clients and Scopes in the Database

OpenIddict requires a database in which to store clients (which it calls 'applications') and scopes. Generally speaking client and scope data comes from configuration, therefore there needs to be a mechanism to get the data from configuration into the database in each environment.

When running the app locally, a hosted service can be used to do this on app startup. The necessary worker service is provided in the `Audacia.Auth.OpenIddict.Seeding` package, and should be called as follows (where `services` is an instance of `IServiceCollection` and `environment` is an instance of `IWebHostEnvironment`):
```csharp
if (environment.IsDevelopment())
{
    services.AddLocalSeeding();
}
```

See [here](#seeding-the-database-in-a-pipeline) for information on setting up database seeding to run in a pipeline. **It is recommended that you configure your pipeline later, after you've got the end-to-end authentication process working locally.**

### ASP.NET Core Identity Configuration

Some claim types must be set in the ASP.NET Core Identity configuration as follows:
```csharp
services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Note the claim types should be constants somewhere, or use the IdentityModel NuGet package
    // with its JwtClaimTypes class
    options.ClaimsIdentity.UserIdClaimType = "sub";
    options.ClaimsIdentity.UserNameClaimType = "email";
    options.ClaimsIdentity.RoleClaimType = "role";
    // More code here if you need it....
});
```

### Adding Additional Claims to Tokens - ___optional___

If you require any custom claims beyond the standard ones issued there are two main mechanisms by which this can be achieved.

#### Additional Claims Provider

The `IAdditionalClaimsProvider<TUser>` interface is designed to provide claims that should be added to tokens based on the information present in the authenticated user (i.e. in the `TUser` instance). This can be done by implementing the `IAdditionalClaimsProvider<TUser>` interface.

#### Profile Service

The `Audacia.Auth.OpenIddict` library also provides an `IProfileService<TUser>` interface, which performs essentially the same role as the `IProfileService` interface that is part of IdentityServer4. If you need to perform logic or make calls to a database or external API, the `GetClaimsAsync` method of `IProfileService<TUser>` is the appropriate place for this.

The provided implementation of `IProfileService<TUser>`, `DefaultProfileService<TUser>`, already adds the claims from `IAdditionalClaimsProvider<TUser>` so you can derive from this base class rather than implement the interface directly.

An example implementation might be:
```csharp
public class CustomProfileService : DefaultProfileService<ApplicationUser>
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

### Events - ___optional___

The `Audacia.Auth.OpenIddict` library implements the same event functionality that IdentityServer supports, where events are raised to an `IEventService`, which persists them to an `IEventSink`. `Audacia.Auth.OpenIddict` raises the following events:
- `TokenIssuedSuccessEvent`
- `TokenIssuedFailureEvent`

You can intercept these events and add custom behaviour by implementing `IEventSink` (see [below](#event-sinks)).

In addition, the following events are provided which can be raised in user code:
- `UserLoginSuccessEvent`.
- `UserLoginFailureEvent`.
- `UserLogoutSuccessEvent`.

#### Event Sinks

Events are persisted using an implementation of `IEventSink`. The default implementation uses `Microsoft.Extensions.Logging.ILogger` to log the event, but a custom implementation can be provided by registering it with the dependency injection container. For example:
```csharp
services.AddEventSink<CustomEventSink>();
```

If you still want all events to be logged using the default behaviour then you can inherit from `DefaultEventSink` to utilise the existing behaviour. For example:
```csharp
public class CustomEventSink : DefaultEventSink
{
    public CustomEventSink(ILogger<DefaultEventSink> logger, IEventSerializer eventSerializer)
        : base(logger, eventSerializer)
    {
    }

    public override virtual Task PersistAsync(AuthEvent authEvent)
    {
        // Custom behaviour here

        return base.PersistAsync(authEvent);
    }
}
```

#### Event Serialization

Events are serialized using an implementation of `IEventSerializer`. The default implementation uses `System.Text.Json`, but a custom implementation can be provided by registering it with the dependency injection container. For example:
```csharp
services.AddEventSerializer<CustomEventSerializer>();
```

### Custom Grant Type Support - ___optional___

Additional grant types can be processed by specifying them in the `CustomGrantTypeClients` property of `OpenIdConnectConfig`, and then by providing an appropriate implementation of `ICustomGrantTypeClaimsPrincipalProvider`. For example, if the grant type of `"saml"` needs to be supported then the configuration and `ICustomGrantTypeClaimsPrincipalProvider` implementation would be something like this:
```json
{
    // ...more config
    "CustomGrantTypeClients": [
        {
            "GrantType": "saml",
            "ClientId": "CustomClient",
            "ClientSecret": "xxxx",
            "ClientScopes": [
                "api"
            ],
            "AccessTokenLifetime": {
                "Value": "60",
                "Type": "Minutes"
            },
            "ClientUris": [
                "https://localhost:1234"
            ],
            "ClientType": "Confidential" // If set to "Confidential", a "ClientSecret" must be provided; the other acceptable value is "Public"
        }
    ]
    // more config...
}
```
```csharp
public class SamlClaimsPrincipalProvider : ICustomGrantTypeClaimsPrincipalProvider
{
    public string GrantType { get; } = "saml";

    public Task<ClaimsPrincipal> GetPrincipalAsync(OpenIddictRequest openIddictRequest)
    {
        // Custom implementation here
    }
}
```

The implementing class must also be registered with the dependency injection system as follows (where `services` is an instance of `IServiceCollection`):
```csharp
services.AddCustomGrantTypeProvider<SamlClaimsPrincipalProvider>();
```

### Accessing Signing and Encryption Credentials - ___optional___

If access to the credentials, e.g. certificates, used to the sign and/or encrypt the tokens is needed then the interfaces `ISigningCredentialsProvider` and `IEncryptionCredentialsProvider` respectively can be used. They are automatically registered with the dependency injection system so will be injected anywhere they are declared as a dependency.

### EF Migrations

Finally, you will need an EF migration to add the necessary OpenIddict tables to the database. Assuming you've created a separate database context for this (`OpenIddictContext`) in an `OpenIddict` folder within your Data/Entity Framework project, the commands would be:

```
Add-Migration InitialOpenIddictMigration -Context OpenIddictDbContext -OutputDir OpenIddict/Migrations
Update-Database -Context OpenIddictDbContext
```

### Testing the Changes

Upon completion of the above steps, you should be able to obtain an access token from the Identity app. You should test both the Client Credentials flow and the Resource Owner Password Credentials flow (assuming your `OpenIdConnectConfig` includes both types of client). This testing can be carried using a tool such as Postman.

It is important to test the Identity app in isolation at this point as it will tell you whether or not the OpenIddict authorization server has been configured correctly.

#### Client Credentials Flow

Assuming the `OpenIdConnectConfig` has a Client credentials client called `ApiClient`, an access token can be obtained by sending a `POST` request to the `/connect/token` endpoint with the following `x-www-form-urlencoded` parameters:
| Key | Value |
| --- | --- |
| grant_type | client_credentials |
| scope | _{name of the scope granted to the client}_ |
| client_id | ApiClient |
| client_secret | _{client secret value}_ |

#### Resource Owner Password Credentials Flow

Assuming the `OpenIdConnectConfig` has a Resource Owner Password Credentials client called `TestAutomationClient`, an access token can be obtained by sending a `POST` request to the `/connect/token` endpoint with the following `x-www-form-urlencoded` parameters:
| Key | Value |
| --- | --- |
| grant_type | password |
| scope | _{name of the scope granted to the client}_ |
| username | _{username of the seeded user}_ |
| password | _{password of the seeded user}_ |
| client_id | TestAutomationClient |
| client_secret | _{client secret value}_ |

## API Changes

Assuming the Identity app can successfully issue tokens, you should now change any API in the system to accept these tokens.

On completion of this section you should test that you can:
- Use the tokens obtained above to make authenticated API calls.

### Install NuGet Packages

Install the following NuGet packages in each API project:
- `OpenIddict.AspNetCore`.

### API Authentication

Any API must use OpenIddict to validate access tokens. This can be achieved using the code below (where `services` is an instance of `IServiceCollection`). This code should replace any existing call to `AddAuthentication()`, and if you are replacing `IdentityServer4`, `AddIdentityServerAuthentication()`. Note the slight difference below depending on whether the Identity app is hosted alongside the API or as a standalone application.

```csharp
services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    })
    .AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(/*Identity app url*/);

        options.AddAudiences(/*Client ID of the API*/);

        // IF OPENIDDICT IS HOSTED IN A SEPARATE 'IDENTITY' APP
        options
            .UseIntrospection()
            // To allow the below, the API will need to be registered as a 'client credentials' client with OpenIddict if it isn't already
            .SetClientId(/*Client ID of the API*/)
            .SetClientSecret(/*Client secret of the API*/);
        // ELSE IF OPENIDDICT IS HOSTED IN THE SAME WEB APP AS THE API
        options.UseLocalServer();

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });
```

### Testing the Changes

Once the API is configured you should try and use an access token previously obtained via the Resource Owner Password Credentials flow to make an authenticated API call (i.e. pass the token as a 'Bearer' token in the Authorization header).

It is important to test API authentication at this point as it will tell you whether the API has been configured correctly and whether scopes and resources have been configured correctly in the `OpenIdConnectConfig`.

## UI Changes

The UI implementation will vary depending on the front-end framework used (e.g. Angular, Vue) and also on the client library being used to handle the OAuth process on the client (e.g. [oidc-client-ts](https://github.com/authts/oidc-client-ts), [angular-auth-oidc-client](https://github.com/damienbod/angular-auth-oidc-client)). You should therefore follow the guidance of your chosen library, however the high-level approach is likely to involve:
- Implementing an `auth-service`, providing functions like `signin`, `signout`, `signin-silent` and `get-token`.
   - The `auth-service` is likely also where the client library is configured with things like the scopes to request when signing in and the client ID of the app.
- Implementing a route (e.g. `signin-callback`) to which the user should be returned after a successful login.
- Adding a static HTML page to which the user should be returned as part of the silent renew process.

### Testing the Changes

Once the UI is configured you should make sure that the following actions work:
- Interactive login via the UI, e.g.
   1. Try and access a route in the UI that requires an authenticated user.
   1. Ensure the user is redirected to the Identity app using the Authorization Code + PKCE flow.
   1. Login and check that the user is successfully redirected back to the UI with an access token.
- Silent renew of an expired token; this can most easily be tested by setting the access token lifetime for the UI client to 2 minutes and checking in the browser's dev tools that a new token is obtained.

## DevOps Pipeline Changes

### Token Signing and Encryption Certificates

In all deployed environments you must provide two certificates: one for signing tokens and one for encrypting tokens. The easiest way to do this is to generate self-signed certificates, which can done by executing the [CreateTokenSigningCert.sh](https://dev.azure.com/audacia/Audacia/_git/Audacia.Build?path=/tools/security/certificates/CreateTokenSigningCert.sh) and [ConvertTokenSigningCertToPfx.sh](https://dev.azure.com/audacia/Audacia/_git/Audacia.Build?path=/tools/security/certificates/ConvertTokenSigningCertToPfx.sh) bash scripts.

Once you have two certificates per environment you must:
- Upload the certificates to the appropriate location (see [here](https://dev.azure.com/audacia/Audacia/_wiki/wikis/Audacia.wiki/4317/Adding-Token-Signing-or-Encryption-Certificates) for instructions).
- Specify the location of the certificates in configuration, if required; they default to `"CurrentUser"`, but this can also be set to `"LocalMachine"`.
- Specify the thumbprints of the certificates in configuration (see [here](#configuration-in-appsettings.json) for more information).

### Seeding the Database in a Pipeline

Your deployment pipeline will have to be modified to seed the database with the necessary OpenIddict configuration (see [here](#seeding-clients-and-scopes-in-the-database))

There are steps in the `Audacia.Build` repo to perform the seeding via a custom .NET tool. Provided you are using the standard OpenIddict entities and managers with either EF Core or EF 6, you can use the steps in either `openiddict-seeding-efcore.yaml` or `openiddict-seeding-ef6.yaml`. With the template functionality available in Azure Pipelines YAML, for EF Core the YAML could look something like this:
```yaml
steps:
  - template: /src/deployment/openiddict/tasks/openiddict-seeding-efcore.yaml@templates
    parameters:
      toolVersion: 'x.x.x' # Specify the version of Audacia.Auth.OpenIddict that you are targeting
      identityProjectBasePath: '$(Pipeline.Workspace)/$(Build.DefinitionName)/MyApp.Identity' # The path to the identity app artifact
      identityProjectName: 'MyApp.Identity'
      openIddictEntitiesKeyType: 'int'
      databaseConnectionStringName: 'MyDatabaseContext'
```

For non-YAML pipelines, the code from the respective steps can be copied into tasks in a classic pipeline.

### Testing the Changes

You should now be able to deploy all the changes made and successfully perform all of the previous testing in a deployed environment.