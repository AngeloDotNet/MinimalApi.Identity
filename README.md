# .NET Modular Dynamic Identity Manager

A set of libraries to easily integrate and extend authentication in ASP.NET Core projects, using ASP.NET Core Identity.

## 🏷️ Description

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

<!--
As an alternative to SQL Server you can use one of these databases:

- PostgreSQL
- MySQL
- SQLite
-->

### Setup

The library is available on [NuGet](https://www.nuget.org/packages/Identity.Module.API), just search for _Identity.Module.API_ in the Package Manager GUI or run the following command in the .NET CLI:

```shell
dotnet add package Identity.Module.API
```

## ⚙️ Configuration

The configuration can be completely managed by adding this section to the _appsettings.json_ file:

> [!WARNING]
>  The library is still under development, so the configuration may change in future updates.

> [!NOTE]
> For migrations you can use a specific project to add to your solution, then configuring the assembly in _ConnectionStrings:MigrationsAssembly_, otherwise leave it blank and the assembly containing the _Program.cs_ class will be used.

```json
"Kestrel": {
    "Limits": {
        "MaxRequestBodySize": 5242880
    }
},
"ConnectionStrings": {
    "DatabaseType": "sqlserver", // Options: "sqlserver"
    "SQLServer": "Data Source=[HOSTNAME];Initial Catalog=IdentityManager;User ID=[USERNAME];Password=[PASSWORD];Encrypt=False",
    "MigrationsAssembly": "MinimalApi.Identity.Migrations.SQLServer"
},
"JwtOptions": {
    "SchemaName": "Bearer",
    "Issuer": "[ISSUER]",
    "Audience": "[AUDIENCE]",
    "SecurityKey": "[SECURITY-KEY]", // Must be 512 characters long
    "ClockSkew": "00:05:00", // Default: 5 minutes
    "AccessTokenExpirationMinutes": 60, // 60 minutes
    "RefreshTokenExpirationMinutes": 60, // 60 minutes
    "RequireUniqueEmail": true,
    "RequireDigit": true,
    "RequiredLength": 8,
    "RequireUppercase": true,
    "RequireLowercase": true,
    "RequireNonAlphanumeric": true,
    "RequiredUniqueChars": 4,
    "RequireConfirmedEmail": true,
    "MaxFailedAccessAttempts": 3,
    "AllowedForNewUsers": true,
    "DefaultLockoutTimeSpan": "00:05:00" // 5 minutes
},
"SmtpOptions": {
    "Host": "smtp.example.org",
    "Port": 25,
    "Security": "StartTls",
    "Username": "Username del server SMTP",
    "Password": "Password del server SMTP",
    "Sender": "MyApplication <noreply@example.org>",
    "MaxRetryAttempts": 10
},
"ApplicationOptions": {
    
    "ErrorResponseFormat": "List"
},
"FeatureFlagsOptions": {
    "EnabledFeatureLicense": true,
    "EnabledFeatureModule": true
},
"HostedServiceOptions": {
    "IntervalAuthPolicyUpdaterMinutes": 5,
    "IntervalEmailSenderMinutes": 1
},
"UsersOptions": {
    "AssignAdminEmail": "admin@example.org",
    "AssignAdminPassword": "StrongPassword",
    "PasswordExpirationDays": 90
},
"ValidationOptions": {
    "MinLength": 3,
    "MaxLength": 50,
    "MinLengthDescription": 5,
    "MaxLengthDescription": 100
}
```

## 🗃️ Database

### Configuration

The library uses Entity Framework Core to manage the database.

The connection string is configured in the `ConnectionStrings` section of the _appsettings.json_ file.

- Database Type: Set via `ConnectionStrings:DatabaseType` (supported values: `sqlserver`)

After setting the type of database you want to use, modify the corresponding connection string.

### Migrations

> [!TIP]
> To update the database schema you need to create migrations, they will be applied automatically at the next application startup.

To create database migrations select `MinimalApi.Identity.Core` as the default project from the drop-down menu in the `Package Manager Console`
and run the command: `Add-Migration MIGRATION-NAME`

> [!NOTE]
> if you use a separate project for migrations (It is recommended to add a reference in the project name to the database used, in this case it is SQL Server), 
> make sure to set the `-Project` parameter to the name of that project.

Example: `Add-Migration InitialMigration -Project MinimalApi.Identity.Migrations.SQLServer`

## 🔰 Feature Flags

🚧 coming soon

## 💡 Usage Examples

> [!WARNING]
> The library is still under development, so the Program.cs configuration may change in future updates.

An example configuration of the Program.cs class is available [here](https://github.com/AngeloDotNet/MinimalApi.Identity/blob/main/IdentityManager.API/Program.cs)

## 🔐 Authentication

This library currently supports the following authentication types:

- JWT Bearer Token


### 🧑‍💼 Administrator Account

🚧 coming soon

<!--
A default administrator account is created automatically with the following configuration:

- Username: Set via `UsersOptions:AssignAdminRoleOnRegistration`
- Password: Set via `AppSettings:AdministratorApiKey`
-->

## 📚 API Reference

See the [documentation](https://github.com/AngeloDotNet/MinimalApi.Identity/tree/main/docs/Endpoints) for a list of all available endpoints.

## 📦 Packages

|Name|Type|Version|
|----|----|-------|
|[Identity.Module.API](https://www.nuget.org/packages/Identity.Module.API)|Main|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.API)](https://www.nuget.org/packages/Identity.Module.API)
|[Identity.Module.AccountManager]()|Dependence|Coming soon|
|[Identity.Module.ClaimsManager]()|Dependence|Coming soon|
|[Identity.Module.Core](https://www.nuget.org/packages/Identity.Module.Core)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.Core)](https://www.nuget.org/packages/Identity.Module.Core)|
|[Identity.Module.EmailManager](https://www.nuget.org/packages/Identity.Module.EmailManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.EmailManager)](https://www.nuget.org/packages/Identity.Module.EmailManager)|
|[Identity.Module.Licenses](https://www.nuget.org/packages/Identity.Module.Licenses)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.Licenses)](https://www.nuget.org/packages/Identity.Module.Licenses)|
|[Identity.Module.ModuleManager]()|Dependence|Coming soon|
|[Identity.Module.PolicyManager](https://www.nuget.org/packages/Identity.Module.PolicyManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.PolicyManager)](https://www.nuget.org/packages/Identity.Module.PolicyManager)|
|[Identity.Module.ProfileManager]()|Dependence|Coming soon|
|[Identity.Module.RolesManager]()|Dependence|Coming soon|
|[Identity.Module.Results]()|Dependence|Coming soon|

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

- [ ] Move the configuration of the claims to a dedicated library
- [ ] Move the configuration of the module to a dedicated library
- [ ] Move the configuration of the profile manager to a dedicated library
- [ ] Move the configuration of the roles to a dedicated library
- [ ] Add CancellationToken to API endpoints (where necessary)
- [X] Move email sending logic to a dedicated library
- [X] Modify email sending logic on a hosted service
- [X] Changing the hosted service type to a background service in Email Manager
- [X] Changing the hosted service type to a background service in Policy Manager
- [ ] Add automatic creation of a default administrator account
- [ ] Replacing exceptions with implementation of operation results 
- [ ] Replacing the hosted service email sender using Coravel jobs
- [ ] Replacing the hosted service authorization policy updater using Coravel jobs
- [ ] Migrate SmtpOptions configuration to database
- [ ] Migrate FeatureFlagsOptions configuration to database
- [ ] Add support for the MySQL database 
- [ ] Add support for the PostgreSQL database 
- [ ] Add support for the SQLite database
- [ ] Add support for the AzureSQL database
- [ ] Add endpoints for two-factor authentication and management
- [ ] Add endpoints for downloading and deleting personal data
- [ ] Add support for multi tenancy
- [ ] Add authentication support from third-party providers (e.g. GitHub, Azure)

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ⭐ Give a Star

Don't forget that if you find this project useful, put a ⭐ on GitHub to show your support and help others discover it.

## 🤝 Contributing

The project is constantly evolving. Contributions are always welcome. Feel free to report issues and submit pull requests to the repository, following the steps below:

1. Fork the repository
2. Create a feature branch (starting from the develop branch)
3. Make your changes
4. Submit a pull requests (targeting develop)

## 🆘 Support

If you have any questions or need help, read [here](https://github.com/AngeloDotNet/MinimalApi.Identity/discussions/1) to find out what to do.
