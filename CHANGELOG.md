# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). Every package of this solution is published to NuGet continuously from `main` and versioned automatically with [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning), and the repository has no release tags, so changes are grouped by month, newest first, instead of by version number. Package versions are shown on [NuGet](https://www.nuget.org/packages/Identity.Module.API).

Routine dependency bumps are summarized, not listed one by one. The full history is in the [commit log](https://github.com/AngeloDotNet/MinimalApi.Identity/commits/main).

## [Unreleased]

## 2026-10

### Changed

- JWT bearer authentication keeps the original claim names of the token by disabling inbound claim mapping, and validates the token issuer and audience against the configured `JwtOptions`.
- Dependencies updated: `Identity.Module.Core` 2.5.261, `Identity.Module.API` 2.5.533, `Identity.Module.EmailManager` 2.5.130, `AWSSDK.S3` 4.0.104.6, `MailKit` 4.18.1 and `FluentValidation`.

## 2026-08

### Changed

- .NET SDK pinned to 9.0.317.
- `.editorconfig` refined for modern C# style: static anonymous functions, implicitly typed lambdas, unbound generic types in `nameof`, index and range operators, and pattern local functions.

## 2026-05 and 2026-06

### Changed

- Roadmap in the README now states that no further development is planned for this .NET 9 solution; work continues in a .NET 10 version ([#328](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/328)).
- `EmailManager` and related NuGet packages upgraded ([#332](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/332)).
- SonarQube workflow no longer runs on a schedule or on push, and can only be started manually ([#327](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/327), [#345](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/345)).

## 2026-04

### Changed

- Migrated the whole solution from .NET 8 to **.NET 9**: every project targets `net9.0`, the NuGet package tags reference `net9`, and the publish workflows build with .NET 9.x ([#316](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/316), [#321](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/321), [#322](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/322)).
- `Directory.Build.props` moved to the `src` folder.
- Entity Framework Core, ASP.NET Core Identity, `MailKit`, `AWSSDK.S3` and all `Identity.Module.*` packages upgraded to versions compatible with .NET 9.
- Coding guidelines for AI assistants updated to C# 13.

### Removed

- Commented-out code and explanatory comments across the API, database, Swagger, shared and dynamic policy code ([#317](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/317)).

## 2026-01 to 2026-03

### Added

- Packages are also published to GitHub Packages, in addition to NuGet.org ([#261](https://github.com/AngeloDotNet/MinimalApi.Identity/pull/261)).

### Changed

- `super-linter` upgraded from 7.2.0 to 8.3.1.
- Maintenance period: mostly dependency updates for the `Identity.Module.*` packages and the .NET SDK.

## 2025-12

### Added

- **Claims** and **Modules** management as separate modules (`ClaimsManager`, `ModuleManager`), with their endpoints and documentation.
- Database provider selection through `DbContextOptionsBuilder` extensions for SQL Server, Azure SQL, PostgreSQL, MySQL and SQLite.
- Dynamic registration of the license endpoints, controlled by the feature flags.
- Swagger filters that hide the license and module endpoints when the feature is disabled.
- `AuthSettings` for the optional protection of the Swagger UI.
- Request and response models for authentication: registration, login, refresh token, forgot and reset password, impersonation.
- Names and tags on the authentication endpoints, to group and describe them in Swagger.
- Example of forwarded headers configuration for applications behind a reverse proxy.
- The workflow that cleans up old workflow runs now also deletes old releases.

### Changed

- `AuthPolicyEndpoints` renamed to `PolicyEndpoints`.
- Account endpoints are mapped automatically.
- README: documentation, structure and roadmap updated.

## 2025-11

### Added

- `CorsOptions` with a configurable policy name, to configure CORS from `appsettings.json`.
- Serilog sink that sends logs to MinIO cloud storage.
- `ValidationOptions` to configure the validation rules.

### Changed

- `Result<T>` improved with new properties and better nullability handling.
- Endpoint handlers for authentication, claims and modules reorganized.

## 2025-10

### Added

- **Roles** management as its own module (`RolesManager`): create, assign, revoke and delete roles, with validators and endpoints.
- Support for **PostgreSQL**, **MySQL** and **SQLite** in addition to SQL Server and Azure SQL.
- **Serilog** configuration with console and rolling file sinks.
- `MinioS3Sink` and `MinioOptions` to write logs to S3-compatible storage such as MinIO.
- `IsRequiredAuth` setting to require basic authentication on the Swagger UI.

## 2025-09

### Added

- **Account** management (`AccountManager`): confirmed change of e-mail address and e-mail confirmation endpoints.
- **Authentication** module (`AuthManager`) that creates the administrator account at startup from `AppSettings`.
- **Dynamic authorization policies**: `DynamicAuthorizationPolicyProvider` builds the policies from the database at runtime.
- **License management**: service, validators, models and endpoints to create, assign, revoke and delete licenses.
- Automatic e-mail generation in `EmailManager`.
- EF Core interceptors for SQL command logging, query performance and auditing of user activity.
- Operation result types: `Result`, `Result<T>`, `PaginatedList<T>`, `FailureReasons` and mapping to ASP.NET Core HTTP responses, with a configurable `ErrorResponseFormat`.
- `SwaggerBasicAuthMiddleware` to protect the Swagger UI with basic authentication, and `SwaggerSettings`.
- Runtime `FeatureFlagsOptions`, `JwtOptions` and `AppSettings` configuration classes.
- `UtcDateTimeConverter` for JSON date and time handling.
- Workflow that updates `CHANGELOG.md` when a tag or release is published.
- `ConfigureAwait(false)` on asynchronous library code.

### Removed

- Background services that regenerated authorization policies, replaced by the dynamic policy provider.
- Central package management through `Directory.Packages.props`.

## 2025-08

### Added

- **E-mail outbox** (`EmailManager`): entities for e-mails, types and statuses with seed data, `MailKit` sender, background sender that processes pending messages on a schedule, `SmtpOptions`, and the initial migration.
- **User profiles** (`ProfileManager`): create, edit, enable and disable profiles, with models, validators and endpoints.
- E-mail confirmation and callback URL generation.
- **Feature flags** to enable or disable the license and module features, with Swagger document filters that follow the flags.
- Migrations project for SQL Server, and support for MySQL, PostgreSQL and SQLite.
- `SwaggerExtensions` for the OpenAPI configuration.
- `EndpointGenerator`, `MessagesApi` and `MessagesExceptions` that centralize route constants and response messages.
- Documentation of the authentication, account, claims and policy endpoints.
- Workflows that publish `Identity.Module.EmailManager` and `Identity.Module.ProfileManager` to NuGet.

### Changed

- Exception handling and service messages unified through `MessagesApi` and `MessagesExceptions`.
- `JwtOptions` extended with the password, lockout and e-mail options.
- NuGet package versions managed centrally for a short time.

## 2025-07

### Added

- **Password reset** flow: `ResetPassword` endpoint, model, validator and service method.
- **Policy management** (`PolicyManager`): policies built from permissions, `PermissionHandler` and `PermissionRequirement`, with endpoints and validators.
- **License** endpoints and `LicenseService`.
- `EmailManager` project.
- `CancellationToken` support in the services and endpoints.
- CORS policy and Swagger endpoint configuration.
- Invariant culture support when parsing `TimeSpan` values.
- `ConstantsConfiguration` and `PolicyMapper`.
- Workflows to publish `Identity.Module.Core` and `Identity.Module.API` to NuGet, to lint the code with super-linter, and to analyze it with SonarQube.
- GitHub Copilot instructions and a stricter `.editorconfig`.

### Changed

- `Identity.Module.Core` consumed as a NuGet package instead of a project reference.
- Exception classes simplified, and the exception middleware reworked to handle each case.
- Service registration and configuration option retrieval refactored.

## 2025-06

### Added

- First version of the solution, on .NET 8: ASP.NET Core Identity with Entity Framework Core (`MinimalApiAuthDbContext`), user and role entities, claims, JWT permission handling, Minimal API endpoints, validators and the exception middleware.
- Custom exception types for bad request, conflict, not found, validation and user errors.
- Service registration extensions and endpoint mapping by assembly.
- Licenses feature.
- NuGet publishing and code linting workflows.

### Changed

- Project renamed from `Modular.Identity.*` to `Identity.Module.*`; the main package is `Identity.Module.API`.

[Unreleased]: https://github.com/AngeloDotNet/MinimalApi.Identity/commits/main
