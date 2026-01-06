# .NET Modular Dynamic Identity Manager

![Visitors](https://api.visitorbadge.io/api/visitors?path=https%3A%2F%2Fgithub.com%2FAngeloDotNet%2FMinimalApi.Identity&label=Visitors&countColor=%23263759)

A set of libraries to easily integrate and extend authentication in ASP.NET Core projects, using ASP.NET Core Identity.

## 🏷️ Introduction

**MinimalApi.Identity** is a dynamic and modular identity manager for managing users, roles, claims and more for access control in Asp.Net Mvc Core and Web API, using .NET 8 Minimal API, Entity Framework Core and relational database (of your choice).

> [!IMPORTANT]
> **This library is still under development of new implementations and in the process of creating the related documentation.**

## 🧩 Features

- **Minimal API**: Built using .NET 8 Minimal API for a lightweight and efficient implementation.
- **Entity Framework Core**: Uses EF Core for data access, making it easy to integrate with your existing database.
- **Modular**: The library is designed to be modular, allowing you to add or remove features as needed.
- **Dynamic**: Supports dynamic management of users, roles, claims, forms, licensing and policies.
- **Flexible Configuration**: Easily configurable via `appsettings.json` to suit your application's needs.
- **Outbox Pattern**: Implement the [transactional outbox pattern](https://microservices.io/patterns/data/transactional-outbox.html) for reliable email sending.

## 🛠️ Installation

### Prerequisites

- .NET 8.0 SDK (latest version)
- SQL Server 2022 Express installed ([setup for Windows](https://www.microsoft.com/it-it/download/details.aspx?id=104781)) or in Docker version ([example](https://github.com/AngeloDotNet/Docker.Database/tree/master/SQL-Server-2022-EXP))

As an alternative to SQL Server you can use one of these databases:

- Azure SQL Database ([setup guide](https://learn.microsoft.com/it-it/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal))
- PostgreSQL - Docker version ([example](https://github.com/AngeloDotNet/Docker.Database/tree/master/Postgres-16))
- MySQL - Docker version ([example](https://github.com/AngeloDotNet/Docker.Database/tree/master/MySQL-Server-8_0_34))
- SQLite

### Setup

The library is available on [NuGet](https://www.nuget.org/packages/Identity.Module.API), just search for _Identity.Module.API_ in the Package Manager GUI or run the following command in the .NET CLI:

```shell
dotnet add package Identity.Module.API
```

## ⚙️ Configuration

The configuration can be completely managed by adding this section to the _appsettings.json_ file:

> [!WARNING]
>  The library is still under development, so the configuration may change in future updates.

A complete example of the configurations in AppSettings.json is available [here](https://github.com/AngeloDotNet/MinimalApi.Identity/blob/main/IdentityManager.API/appsettings.json).

## 🗃️ Database

### Configuration

The library uses Entity Framework Core to manage the database.

The connection string is configured in the AppSettings section of the appsettings.json file, while the database type is configured in the Program.cs class.

After configuring the Program.cs class, modify the connection string for the corresponding database.

### Migrations

> [!TIP]
> To update the database schema you need to create migrations, they will be applied automatically at the next application startup.

To create database migrations select `MinimalApi.Identity.Core` as the default project from the drop-down menu in the `Package Manager Console`
and run the command: `Add-Migration MIGRATION-NAME`

Example: `Add-Migration InitialMigration -Project MinimalApi.Identity.Migrations.SQLServer`

> [!NOTE]
> if you use a separate project for migrations (It is recommended to add a reference in the project name to the database used, in this case it is SQL Server), 
> make sure to set the `-Project` parameter to the name of that project.

## 📎 Swagger / OpenAPI

It is possible to protect access to the Swagger UI with the following configuration in SwaggerSettings:

- RequiredAuth: set via `AuthSettings:IsRequired` (supported values: `true`, `false`)
- Username: set via `AuthSettings:Username`
- Password: set via `AuthSettings:Password`

You can manage the state of the Swagger UI with the following configuration:

- Enable/Disable Swagger UI: set via `SwaggerSettings:IsEnabled` (supported values: `true`, `false`)

<!--
## 🔰 Feature Flags

🚧 coming soon
-->

## 💡 Usage Examples

> [!WARNING]
> The library is still under development, so the Program.cs configuration may change in future updates.

A complete example of the Program.cs class is available [here](https://github.com/AngeloDotNet/MinimalApi.Identity/blob/main/IdentityManager.API/Program.cs).

## 🔐 Authentication

The following authentication types are currently supported:

- JWT Bearer Token

### 🧑‍💼 Administrator Account

A default administrator account is created automatically with the following configuration:

- Email: set via `AppSettings:AssignAdminEmail`
- Username: set via `AppSettings:AssignAdminUsername`
- Password: set via `AppSettings:AssignAdminPassword`

<!--
## 📚 API Reference

See the [documentation](https://github.com/AngeloDotNet/MinimalApi.Identity/tree/main/docs/Endpoints) for a list of all available endpoints.
-->

## 🏆 Badges

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=alert_status)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=bugs)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=code_smells)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=progetti-2025_minimalapi-identity)

[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity) 
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=security_rating)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=progetti-2025_minimalapi-identity)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity) 
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=progetti-2025_minimalapi-identity&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=progetti-2025_minimalapi-identity)

## 🗺️ Roadmap

- [ ] Migrate solution to .NET 9
- [ ] Migrate SwaggerSettings configuration to database
- [ ] Migrate SmtpOptions configuration to database
- [ ] Replacing the hosted service email sender using Coravel jobs
- [ ] Migrate solution to .NET 10
- [ ] Replacing exceptions with implementation of operation results
- [ ] Add support for multi tenancy
- [ ] Align endpoints with the updated version of ASP.NET Core Identity (including endpoints for two-factor authentication and management, downloading and deleting personal data)
- [ ] Code Review and Refactoring
- [ ] Update documentation

## 🚀 Future implementations

- [ ] Change the entity ID type from INT to GUID
- [ ] Make the ID entity type dynamic, so that it can accept both INT and GUID at runtime
- [ ] Add authentication support from third-party providers (e.g. Auth0, KeyCloak, GitHub, Azure)
- [ ] Migrate FeatureFlagsOptions to Feature Management (package Microsoft.FeatureManagement)

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ⭐ Give a Star

Don't forget that if you find this project helpful, please give it a ⭐ on GitHub to show your support and help others discover it.

## 🤝 Contributing

The project is constantly evolving. Contributions are always welcome. Feel free to report issues and submit pull requests to the repository, following the steps below:

1. Fork the repository
2. Create a feature branch (starting from the develop branch)
3. Make your changes
4. Submit a pull requests (targeting develop)

## 🆘 Support

If you have any questions or need help, read [here](https://github.com/AngeloDotNet/MinimalApi.Identity/discussions/1) to find out what to do.
