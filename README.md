# .NET Modular Dynamic Identity Manager

Modular dynamic identity manager for users, roles, claims and more for access control in Asp.Net Mvc Core and Web API, using .NET 8 Minimal API, Entity Framework Core and SQL Server.

> [!IMPORTANT]
> **This library is still under development of new implementations and in the process of creating the related documentation.**

## 📎 Table of Contents
- [Table of Contents](#-table-of-contents)
- [Features](#-features)
- [Installation](#%EF%B8%8F-installation)
- [Configuration](#%EF%B8%8F-configuration)
- [Usage Example](#-usage-examples)
- [API Reference](#-api-reference)
- [ToDo](#%EF%B8%8F-todo)
- [Packages](#-packages)
- [Badges](#-badges)
- [License](#-license)
- [Give a Star](#-give-a-star)
- [Contributing](#-contributing)
- [Support](#-support)

## 🧩 Features

- **Minimal API**: Built using .NET 8 Minimal API for a lightweight and efficient implementation.
- **Entity Framework Core**: Uses EF Core for data access, making it easy to integrate with your existing database.
- **Modular**: The library is designed to be modular, allowing you to add or remove features as needed.
- **Dynamic**: Supports dynamic management of users, roles, claims and policies.
- **Flexible Configuration**: Easily configurable via `appsettings.json` to suit your application's needs.

## 🛠️ Installation

### Prerequisites

- .NET 8.0 SDK (latest version)
- SQL Server 2022 Express installed ([setup for Windows](https://www.microsoft.com/it-it/download/details.aspx?id=104781)) or in Docker version ([example](https://github.com/AngeloDotNet/Docker.Database/tree/master/SQL-Server-2022-EXP))

### Setup

The library is available on [NuGet](https://www.nuget.org/packages/Identity.Module.API), just search for _Identity.Module.API_ in the Package Manager GUI or run the following command in the .NET CLI:

```shell
dotnet add package Identity.Module.API
```

## ⚙️ Configuration

The configuration can be completely managed by adding this section to the _appsettings.json_ file:

> [!WARNING]
>  The library is still under development, so the configuration may change in future updates.

```json
{
    "JwtOptions": {
        "Issuer": "[ISSUER]",
        "Audience": "[AUDIENCE]",
        "SecurityKey": "[SECURITY-KEY-512-CHAR]",
        "AccessTokenExpirationMinutes": 60,
        "RefreshTokenExpirationMinutes": 60,
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
        "DefaultLockoutTimeSpan": "00:05:00"
    },
    "SmtpOptions": {
        "Host": "smtp.example.org",
        "Port": 25,
        "Security": "StartTls",
        "Username": "Username del server SMTP",
        "Password": "Password del server SMTP",
        "Sender": "MyApplication <noreply@example.org>",
        "SaveEmailSent": true
    },
    "UsersOptions": {
        "AssignAdminRoleOnRegistration": "admin@example.org",
        "PasswordExpirationDays": 90
    },
    "ValidationOptions": {
        "MinLength": 3,
        "MaxLength": 50,
        "MinLengthDescription": 5,
        "MaxLengthDescription": 100
    },
    "HostedServiceOptions": {
        "IntervalAuthPolicyUpdaterMinutes": 5
    },
    "ApplicationOptions": {
        "MigrationsAssembly": "MinimalApi.Identity.Migrations" //Default for migrations assembly is the project that contains the Program.cs class
    },
    "ConnectionStrings": {
        "DatabaseType": "sqlserver",
        "SQLServer": "Data Source=[HOSTNAME];Initial Catalog=[DATABASE];User ID=[USERNAME];Password=[PASSWORD];Encrypt=False"
    }
}
```

> [!NOTE]
> For migrations you can use a specific project to add to your solution, then configuring the assembly in _ApplicationOptions:MigrationsAssembly_, otherwise leave it blank and the assembly containing the _Program.cs_ class will be used.

<!--
## 🚀 Getting Started

Coming soon stay tuned

## 🔐 Authentication

This library currently supports the following authentication types:

- JWT Bearer Token

### 🧑‍💼 Administrator Account

A default administrator account is created automatically with the following configuration:

- Username: Set via `UsersOptions:AssignAdminRoleOnRegistration`
- Password: Set via `AppSettings:AdministratorApiKey`
-->

## 💡 Usage Examples

> [!WARNING]
>  The library is still under development, so the Program.cs configuration may change in future updates.

A practical example of Program.cs configuration is available [here](https://github.com/AngeloDotNet/MinimalApi.Identity/blob/main/IdentityManager.API/Program.cs)

## 📚 API Reference

See the [documentation](https://github.com/AngeloDotNet/MinimalApi.Identity/tree/main/docs/Endpoints) for a list of all available endpoints.

## 🏗️ ToDo

- [ ] Move the configuration of the claims to a dedicated library
- [ ] Move the configuration of the module to a dedicated library
- [ ] Move the configuration of the profile manager to a dedicated library
- [ ] Move the configuration of the roles to a dedicated library
- [ ] Add CancellationToken to API endpoints (where necessary)
- [ ] Move email sending logic (with improvements) to a hosted service
- [ ] Add email sending implementation to AccountQuery class
- [ ] Add automatic creation of a default administrator account
- [ ] Replacing exceptions with implementation of operation results 
- [ ] Replacing the hosted service email sender using Coravel jobs
- [ ] Replacing the hosted service authorization policy updater using Coravel jobs
- [ ] Add support for relational databases other than MS SQLServer (e.g. MySQL and PostgreSQL)
- [ ] Add endpoints for two-factor authentication and management
- [ ] Add endpoints for downloading and deleting personal data
- [ ] Add support for multi tenancy
- [ ] Add authentication support from third-party providers (e.g. GitHub, Azure)

## 📦 Packages

Main packages:

|Package Name|Version|Downloads|
|------------|-------|---------|
|[Identity.Module.API](https://www.nuget.org/packages/Identity.Module.API)|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.API)](https://www.nuget.org/packages/Identity.Module.API)|[![Nuget](https://img.shields.io/nuget/dt/Identity.Module.Api)](https://www.nuget.org/packages/Identity.Module.Api/)|

Optional packages:

|Package Name|Version|Downloads|
|------------|-------|---------|
|[Identity.Module.Licenses](https://www.nuget.org/packages/Identity.Module.Licenses)|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.Licenses)](https://www.nuget.org/packages/Identity.Module.Licenses)|[![Nuget](https://img.shields.io/nuget/dt/Identity.Module.Licenses)](https://www.nuget.org/packages/Identity.Module.Licenses/)|

Dependencies Packages:

|Package Name|Version|Downloads|
|------------|-------|---------|
|[Identity.Module.AccountManager]()|Coming soon|
|[Identity.Module.ClaimsManager]()|Coming soon||
|[Identity.Module.Core](https://www.nuget.org/packages/Identity.Module.Core)|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.Core)](https://www.nuget.org/packages/Identity.Module.Core)|[![Nuget](https://img.shields.io/nuget/dt/Identity.Module.Core)](https://www.nuget.org/packages/Identity.Module.Core/)|
|[Identity.Module.EmailManager]()|Coming soon||
|[Identity.Module.ModuleManager]()|Coming soon||
|[Identity.Module.PolicyManager](https://www.nuget.org/packages/Identity.Module.PolicyManager)|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.PolicyManager)](https://www.nuget.org/packages/Identity.Module.PolicyManager)|[![Nuget](https://img.shields.io/nuget/dt/Identity.Module.PolicyManager)](https://www.nuget.org/packages/Identity.Module.PolicyManager/)|
|[Identity.Module.ProfileManager]()|Coming soon||
|[Identity.Module.RolesManager]()|Coming soon||

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

If you have any questions or need help, you can add a new thread [here](https://github.com/AngeloDotNet/MinimalApi.Identity/discussions/1).
