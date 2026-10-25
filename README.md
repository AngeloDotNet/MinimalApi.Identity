<h1 align="center">MinimalApi.Identity</h1>

<p align="center">
  A modular identity manager for ASP.NET Core Minimal APIs: users, roles, claims, dynamic policies, licenses and modules on top of ASP.NET Core Identity, Entity Framework Core and JWT.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Identity.Module.API"><img alt="NuGet" src="https://img.shields.io/nuget/v/Identity.Module.API?style=flat-square&logo=nuget" /></a>
  <a href="https://www.nuget.org/packages/Identity.Module.API"><img alt="NuGet downloads" src="https://img.shields.io/nuget/dt/Identity.Module.API?style=flat-square&logo=nuget" /></a>
  <a href="https://github.com/AngeloDotNet/MinimalApi.Identity/actions/workflows/publish.yml"><img alt="Publish" src="https://img.shields.io/github/actions/workflow/status/AngeloDotNet/MinimalApi.Identity/publish.yml?style=flat-square&label=publish" /></a>
  <img alt=".NET 9" src="https://img.shields.io/badge/.NET-9-512BD4?style=flat-square&logo=dotnet" />
  <img alt="Minimal API" src="https://img.shields.io/badge/ASP.NET%20Core-Minimal%20API-512BD4?style=flat-square" />
  <a href="LICENSE"><img alt="License: MIT" src="https://img.shields.io/badge/license-MIT-green?style=flat-square" /></a>
</p>

<p align="center">
  <a href="#overview">Overview</a> •
  <a href="#features">Features</a> •
  <a href="#getting-started">Getting started</a> •
  <a href="#configuration">Configuration</a> •
  <a href="#database-and-migrations">Database</a> •
  <a href="#authentication-and-authorization">Authorization</a> •
  <a href="#api-reference">API reference</a> •
  <a href="#project-structure">Project structure</a> •
  <a href="#contributing">Contributing</a> •
  <a href="#license">License</a>
</p>

---

## Overview

MinimalApi.Identity is a set of NuGet packages that adds a complete, ready-to-use identity API to an ASP.NET Core application.

- Built on **ASP.NET Core Identity**, **Entity Framework Core** and **.NET Minimal APIs**, with **JWT bearer** authentication.
- **Modular**: each area (authentication, accounts, roles, claims, policies, profiles, e-mail, licenses, modules) is its own package, and the optional ones can be switched on or off from configuration.
- **Dynamic**: roles, claims, authorization policies, licenses and modules are data, managed through the API at runtime instead of being hard-coded.
- Works with the relational database of your choice: SQL Server, Azure SQL, PostgreSQL, MySQL or SQLite.

The package you reference is [`Identity.Module.API`](https://www.nuget.org/packages/Identity.Module.API), which brings in all the others. The `IdentityManager.API` project in this repository is a complete sample host.

## Features

### Authentication and accounts

- **JWT bearer tokens** with refresh tokens, configurable lifetime, issuer, audience and clock skew.
- **Register, login, logout, refresh token** and **forgot / reset password** flows.
- **User impersonation** for support and administration scenarios.
- **E-mail confirmation** on registration and **confirmed e-mail change**.
- Password rules, lockout and unique e-mail enforced through the ASP.NET Core Identity options you set in `JwtOptions`.

### Authorization

- **Default roles** (`Admin`, `PowerUser`, `User`) and full role management: create, assign, revoke, delete.
- **Claims** that can be created, assigned to and revoked from users.
- **Dynamic authorization policies** stored in the database, each one a set of permissions, resolved at runtime by a custom policy provider.
- **Permission model** with read and write levels per area, checked on every management endpoint.
- **Secure by default**: a fallback policy requires an authenticated user for any endpoint that does not say otherwise.

### Optional modules

- **Licenses** and **modules** can be created and assigned to users. Both are switched on or off with the `EnabledFeatureLicense` and `EnabledFeatureModule` flags, and their endpoints are only mapped when enabled.

### E-mail with the outbox pattern

- E-mails are first stored in the database and then sent by a background service, following the [transactional outbox pattern](https://microservices.io/patterns/data/transactional-outbox.html), so a temporary SMTP failure does not lose messages.
- Each message has a status (`Pending`, `Sent`, `Failed`, `Cancelled`) and is retried up to a configurable number of attempts. Sending uses [MailKit](https://github.com/jstedfast/MailKit).

### Operations

- **Swagger / OpenAPI** with optional HTTP basic authentication in front of the UI.
- **Serilog** with console and rolling file sinks, plus an optional sink that ships logs to an S3-compatible storage such as [MinIO](https://min.io/).
- **EF Core interceptors** for logging, performance and auditing, plus a security interceptor that blocks `DELETE` statements without a `WHERE` clause.
- **Validation** with FluentValidation and a choice of two error response formats (`Default` or `List`), plus `ProblemDetails` for errors.
- **Configurable CORS**, request size limit and lowercase routes.

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A supported database (SQL Server 2022 Express, Azure SQL, PostgreSQL, MySQL or SQLite). To run a database locally in Docker, see the [examples](https://github.com/AngeloDotNet/Docker.Database).
- An SMTP server, because e-mail confirmation is required by default

### Install

```bash
dotnet add package Identity.Module.API
```

### Register the services

This is the registration used by the sample host, without the optional MinIO log sink. The complete file is [`IdentityManager.API/Program.cs`](IdentityManager.API/Program.cs).

```csharp
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Services;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Bind the configuration sections
var appSettings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new();
var jwtOptions = builder.Services.ConfigureAndGet<JwtOptions>(builder.Configuration, nameof(JwtOptions)) ?? new();
var swaggerSettings = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new();
var corsOptions = builder.Services.ConfigureAndGet<CorsOptions>(builder.Configuration, nameof(CorsOptions)) ?? new();
builder.Services.ConfigureAndGet<SmtpOptions>(builder.Configuration, nameof(SmtpOptions));

var activeModules = RegisterServicesExtensions.ReadFeatureFlags(appSettings);
var errorResponseFormat = ErrorResponseFormat.List;   // or ErrorResponseFormat.Default

// Identity, JWT, database, CORS, validation and every module
builder.Services.AddRegisterDefaultServices<MinimalApiAuthDbContext>(options =>
{
    options.Configuration = builder.Configuration;
    options.TypeDatabase = DatabaseType.SQLServer;      // AzureSQL, PostgreSQL, MySQL or SQLite
    options.ErrorResponseFormat = errorResponseFormat;
    options.ApplicationSettings = appSettings;
    options.JwtSettings = jwtOptions;
    options.CorsSettings = corsOptions;
    options.ActiveModules = activeModules;
    options.MigrationsAssembly = "MinimalApi.Identity.Migrations.SQLServer";
});

// Registers every class whose name ends with "Service" (keep IAuthService in the list)
builder.Services.AddRegisterServices(options =>
{
    options.Interfaces = [typeof(IAuthService)];
    options.StringEndsWith = "Service";
    options.Lifetime = ServiceLifetime.Transient;
});

builder.Services.AddAuthorization(options => options.AddDefaultSecurityOptions());

var app = builder.Build();

// Creates the database or applies the pending migrations
await RegisterServicesExtensions.ConfigureDatabaseAsync(app.Services);

app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseMiddleware<MinimalApiExceptionMiddleware>(errorResponseFormat);

if (swaggerSettings is { IsEnabled: true })
{
    if (swaggerSettings.AuthSettings?.IsRequired is true)
    {
        app.UseMiddleware<SwaggerBasicAuthMiddleware>();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API v1"));
}

app.UseRouting();
app.UseCors(corsOptions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.UseMapEndpoints(activeModules);
await app.RunAsync();
```

> [!NOTE]
> If the application runs behind a reverse proxy, enable `UseForwardedHeaders` and configure `KnownProxies`, as shown in the sample `Program.cs`.

### Run the sample host

1. Open `IdentityManager.API/appsettings.json` and set the connection string, `JwtOptions`, `SmtpOptions` and the administrator account (see [Configuration](#configuration)).
2. Run it:

   ```bash
   dotnet run --project IdentityManager.API
   ```

3. Open the Swagger UI at `/swagger` and log in with the administrator account.

## Configuration

Everything is configured in `appsettings.json`. A complete example is in [`IdentityManager.API/appsettings.json`](IdentityManager.API/appsettings.json).

> [!IMPORTANT]
> Never commit real secrets. Keep connection strings, the JWT security key, SMTP credentials and the administrator password in [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or in environment variables, and replace every placeholder of the sample file.

### Application settings (`AppSettings`)

| Setting | Default | Description |
| --- | --- | --- |
| `AssignAdminUsername` / `AssignAdminEmail` / `AssignAdminPassword` | none | Credentials of the administrator created automatically at startup. |
| `PasswordExpirationDays` | `90` | Days after which a password expires. |
| `IntervalEmailSenderMinutes` | `5` | How often the background service looks for e-mails to send. |
| `EnabledFeatureLicense` | `true` | Enables the licenses module and its endpoints. |
| `EnabledFeatureModule` | `true` | Enables the modules module and its endpoints. |
| `ValidateMinLength` / `ValidateMaxLength` | `3` / `50` | Length limits applied to names in the validators. |
| `ValidateMinLengthDescription` / `ValidateMaxLengthDescription` | `5` / `100` | Length limits applied to descriptions in the validators. |

### JWT and password rules (`JwtOptions`)

| Setting | Example | Description |
| --- | --- | --- |
| `Issuer` / `Audience` | `[ISSUER]` / `[AUDIENCE]` | Token issuer and audience, both validated. |
| `SecurityKey` | `[SECURITY-KEY]` | Signing key. Use a long random secret (64 bytes are recommended for HS512). |
| `ClockSkew` | `00:05:00` | Tolerance applied to the token lifetime. |
| `AccessTokenExpirationMinutes` | `60` | Lifetime of the access token. |
| `RefreshTokenExpirationMinutes` | `60` | Lifetime of the refresh token. |
| `RequireUniqueEmail` | `true` | One account per e-mail address. |
| `RequireConfirmedEmail` | `true` | Users must confirm their e-mail before signing in. |
| `RequireDigit`, `RequireUppercase`, `RequireLowercase`, `RequireNonAlphanumeric` | `true` | Password complexity rules. |
| `RequiredLength` / `RequiredUniqueChars` | `8` / `4` | Minimum password length and number of distinct characters. |
| `MaxFailedAccessAttempts` | `3` | Failed sign-ins before the account is locked. |
| `AllowedForNewUsers` | `true` | Lockout applies to new users too. |
| `DefaultLockoutTimeSpan` | `00:05:00` | How long an account stays locked. |

### E-mail (`SmtpOptions`)

| Setting | Example | Description |
| --- | --- | --- |
| `Host` / `Port` | `smtp.example.org` / `25` | SMTP server. |
| `Security` | `StartTls` | One of `None`, `Auto`, `SslOnConnect`, `StartTls`, `StartTlsWhenAvailable`. |
| `Username` / `Password` | none | SMTP credentials. |
| `Sender` | `MyApplication <noreply@example.org>` | Sender shown to the recipients. |
| `MaxRetryAttempts` | `10` | Attempts before a message is cancelled. |

### Swagger, CORS and logging

| Section | Setting | Description |
| --- | --- | --- |
| `SwaggerSettings` | `IsEnabled` | Shows or hides the Swagger UI. |
| `SwaggerSettings:AuthSettings` | `IsRequired`, `Username`, `Password` | Protects the Swagger UI with HTTP basic authentication. |
| `CorsOptions` | `PolicyName`, `AllowAnyOrigin`, `AllowAnyMethod`, `AllowAnyHeader` | Enables everything for development. |
| `CorsOptions` | `AllowedOrigins`, `AllowedMethods`, `AllowedHeaders` | Explicit allow lists, used when the matching `AllowAny...` flag is `false`. |
| `Serilog` | standard Serilog configuration | Console and rolling file (`Logs/log.txt`, 7 files, warnings and above) in the sample. |
| `MinioOptions` | `Endpoint`, `AccessKey`, `SecretKey`, `BucketName`, `LogObjectKey` | Optional S3-compatible log sink. It is enabled only when both `AccessKey` and `SecretKey` are set. |
| `Kestrel:Limits` | `MaxRequestBodySize` | Maximum request size (5 MB in the sample). |

> [!WARNING]
> The sample CORS configuration allows any origin, method and header. Restrict it before going to production.

## Database and migrations

The connection string is read from the `ConnectionStrings` section, using the name of the selected provider: `SQLServer`, `AzureSQL`, `PostgreSQL`, `MySQL` or `SQLite`.

| `DatabaseType` | Connection string name |
| --- | --- |
| `SQLServer` | `SQLServer` |
| `AzureSQL` | `AzureSQL` |
| `PostgreSQL` | `PostgreSQL` |
| `MySQL` | `MySQL` |
| `SQLite` | `SQLite` |

**At startup**, `ConfigureDatabaseAsync` does one of two things:

- if the database cannot be reached or does not exist, it creates it with `EnsureCreated`;
- otherwise it applies any pending migration.

The repository ships migrations for **SQL Server** in `src/MinimalApi.Identity.Migrations.SQLServer`. For another provider, create a migrations project for it and pass its assembly name in `options.MigrationsAssembly`.

To add a migration, either from the Package Manager Console (default project `MinimalApi.Identity.Migrations.SQLServer`):

```powershell
Add-Migration MigrationName
```

or from the .NET CLI, from the repository root:

```bash
dotnet ef migrations add MigrationName \
  --project src/MinimalApi.Identity.Migrations.SQLServer \
  --startup-project IdentityManager.API \
  --context MinimalApiAuthDbContext
```

> [!TIP]
> Migrations are applied automatically the next time the application starts.

## Authentication and authorization

### Administrator account

At startup an administrator is created from `AppSettings:AssignAdminUsername`, `AssignAdminEmail` and `AssignAdminPassword`, and assigned the `Admin` role. Change the password immediately and do not keep the sample value.

### Default roles

`Admin`, `PowerUser` and `User`.

### Permissions

Management endpoints are protected by permission policies. Each area has a read and a write permission:

| Area | Read | Write |
| --- | --- | --- |
| Authorization policies | `AuthPolicyRead` | `AuthPolicyWrite` |
| Claims | `ClaimRead` | `ClaimWrite` |
| Licenses | `LicenzaRead` | `LicenzaWrite` |
| Modules | `ModuloRead` | `ModuloWrite` |
| Profiles | `ProfiloRead` | `ProfiloWrite` |
| Roles | `RuoloRead` | `RuoloWrite` |

A permission is a claim of type `Permission` assigned to a user (`POST /api/claims/assign-claim`). A policy groups one or more permissions (`POST /api/policy/create-policy`), and a request is authorized only when the user holds **all** the permissions of the policy.

### Calling the API

Log in to obtain an access token, then send it in the `Authorization` header:

```http
POST /api/auth/login
Content-Type: application/json

{ "username": "admin", "password": "<password>", "rememberme": false }
```

```http
GET /api/roles
Authorization: Bearer <accessToken>
```

When the access token expires, exchange it with `POST /api/auth/refresh-token`, sending the `accessToken` and the `refreshToken`.

## API reference

All routes are lower case and start with `/api`. Parameters of each endpoint are documented in [`docs/Endpoints`](docs/Endpoints) and in the Swagger UI.

| Group | Method and route | Access |
| --- | --- | --- |
| **Authentication** `/api/auth` | `POST /register`, `POST /login`, `POST /refresh-token`, `POST /logout`, `POST /forgot-password`, `POST /reset-password` | Public |
| | `POST /impersonate-user` | Authenticated |
| **Account** `/api/account` | `GET /confirm-email/{userId}/{token}`, `GET /confirm-email-change/{userId}/{email}/{token}` | Public |
| | `POST /change-email` | Authenticated |
| **Roles** `/api/roles` | `GET /` | `RuoloRead` |
| | `POST /create-role`, `POST /assign-role`, `DELETE /revoke-role`, `DELETE /delete-role` | `RuoloWrite` |
| **Claims** `/api/claims` | `GET /` | `ClaimRead` |
| | `POST /create-claim`, `POST /assign-claim`, `DELETE /revoke-claim`, `DELETE /delete-claim` | `ClaimWrite` |
| **Policies** `/api/policy` | `GET /` | `AuthPolicyRead` |
| | `POST /create-policy`, `DELETE /delete-policy` | `AuthPolicyWrite` |
| **Profiles** `/api/profiles` | `GET /`, `GET /{userId}`, `POST /change-enable-profile` | `ProfiloRead` |
| | `PUT /edit-profile` | `ProfiloWrite` |
| **Licenses** `/api/licenses` * | `GET /` | `LicenzaRead` |
| | `POST /create-license`, `POST /assign-license`, `DELETE /revoke-license`, `DELETE /delete-license` | `LicenzaWrite` |
| **Modules** `/api/modules` * | `GET /` | `ModuloRead` |
| | `POST /create-module`, `POST /assign-module`, `DELETE /revoke-module`, `DELETE /delete-module` | `ModuloWrite` |

\* Mapped only when `EnabledFeatureLicense` / `EnabledFeatureModule` is `true`.

## Project structure

```
IdentityManager.API/                    Sample host (Program.cs, appsettings.json)
docs/Endpoints/                         Endpoint documentation
src/
├── MinimalApi.Identity.API/            Identity.Module.API          Entry point: registration, auth endpoints, Swagger, middleware
├── MinimalApi.Identity.Core/           Identity.Module.Core         Entities, DbContext, options, authorization, validators
├── MinimalApi.Identity.Shared/         Identity.Module.Shared       Result types, EF Core interceptors, Serilog and MinIO sink
├── MinimalApi.Identity.AuthManager/    Identity.Module.AuthManager  Administrator seeding at startup
├── MinimalApi.Identity.AccountManager/ Identity.Module.AccountManager  E-mail confirmation and change
├── MinimalApi.Identity.RolesManager/   Identity.Module.RolesManager    Roles
├── MinimalApi.Identity.ClaimsManager/  Identity.Module.ClaimsManager   Claims
├── MinimalApi.Identity.PolicyManager/  Identity.Module.PolicyManager   Dynamic authorization policies
├── MinimalApi.Identity.ProfileManager/ Identity.Module.ProfileManager  User profiles
├── MinimalApi.Identity.EmailManager/   Identity.Module.EmailManager    E-mail outbox and background sender
├── MinimalApi.Identity.LicenseManager/ Identity.Module.LicenseManager  Licenses (optional)
├── MinimalApi.Identity.ModuleManager/  Identity.Module.ModuleManager   Modules (optional)
└── MinimalApi.Identity.Migrations.SQLServer/   EF Core migrations for SQL Server
```

Every package under `src/` is versioned and published to NuGet independently by its own workflow in `.github/workflows`.

## Roadmap

No further development is currently planned for this .NET 9 solution. Work continues in a .NET 10 version, in a dedicated repository that is temporarily private:

- [ ] Migrate to .NET 10 (in progress)
- [ ] Replace the e-mail hosted service with Coravel jobs
- [ ] Add multi-tenancy
- [ ] Align the endpoints with the latest ASP.NET Core Identity, including two-factor authentication and personal data download and deletion
- [ ] Add external authentication providers: Auth0, Keycloak, GitHub and Azure
- [ ] Code review and refactoring
- [X] Update the documentation

See the [changelog](CHANGELOG.md) for what changed in each release.

## Contributing

The project is constantly evolving and contributions are always welcome. Feel free to report issues and submit pull requests:

1. Fork the repository
2. Create a feature branch (starting from the `develop` branch)
3. Make your changes
4. Submit a pull request (targeting `develop`)

## Support

If you have any questions or need help, read [this discussion](https://github.com/AngeloDotNet/MinimalApi.Identity/discussions/1) to find out what to do.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Give a star

If you find this project helpful, please give it a ⭐ on GitHub to show your support and help others discover it.
