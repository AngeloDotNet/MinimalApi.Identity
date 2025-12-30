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

```json
"Kestrel": {
    "Limits": {
        "MaxRequestBodySize": 5242880
    }
},
"Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": "Warning",
    "WriteTo": [
        {
            "Name": "Console",
            "Args": {
                "outputTemplate": "{Timestamp:HH:mm:ss}\t{Level:u3}\t{SourceContext}\t{Message}{NewLine}{Exception}"
            }
        },
        {
            "Name": "File",
            "Args": {
                "path": "Logs/log.txt",
                "rollingInterval": "Day",
                "retainedFileCountLimit": 7,
                "restrictedToMinimumLevel": "Warning",
                "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog"
            }
        }
        // The custom MinioS3Sink sink must be added via code (not here)
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
        "Application": "MinimalApi.Identity"
    }
},
"ConnectionStrings": {
    "AzureSQL": "Server=tcp:[SERVER].database.windows.net,1433;Initial Catalog=IdentityManager;Persist Security Info=False;User ID=[USERNAME];Password=[PASSWORD];MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "MySQL": "Server=[HOSTNAME];Port=3306;Database=IdentityManager;Uid=[USERNAME];Pwd=[PASSWORD]",
    "PostgreSQL": "Host=[HOSTNAME];Port=5432;Database=IdentityManager;Username=[USERNAME];Password=[PASSWORD];Include Error Detail=true",
    "SQLite": "Data Source=Data/IdentityManager.db", // OR "SQLite": "Data Source=MinimalApi.Identity.db;Cache=Shared"
    "SQLServer": "Data Source=[HOSTNAME];Initial Catalog=IdentityManager;User ID=[USERNAME];Password=[PASSWORD];Encrypt=False"
},
"JwtOptions": {
    "Issuer": "[ISSUER]",
    "Audience": "[AUDIENCE]",
    "SecurityKey": "[SECURITY-KEY]", // Must be 512 characters long
    "ClockSkew": "00:05:00", 
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
    "MaxRetryAttempts": 10
},
"AppSettings": {
    "DatabaseType": "sqlserver",
	"MigrationsAssembly": "MinimalApi.Identity.Migrations.SQLServer",
    "AssignAdminUsername": "admin",
    "AssignAdminEmail": "admin@example.org",
    "AssignAdminPassword": "StrongPassword",
    "PasswordExpirationDays": 90,
    "IntervalEmailSenderMinutes": 5,
    "ErrorResponseFormat": "List",
    "EnabledFeatureLicense": true,
    "EnabledFeatureModule": true,
    "ValidateMinLength": 3,
    "ValidateMaxLength": 50,
    "ValidateMinLengthDescription": 5,
    "ValidateMaxLengthDescription": 100
},
"SwaggerSettings": {
    "IsEnabled": true,
    "AuthSettings": {
        "IsRequired": false,
        "Username": "admin",
        "Password": "StrongPassword"
    }
},
"MinioOptions": {
    "Endpoint": "http://127.0.0.1:9000",
    "AccessKey": "",
    "SecretKey": "",
    "BucketName": "logs",
    "LogObjectKey": "serilog-demo.json"
},
"CorsOptions": {
    "PolicyName": "DefaultCorsPolicy",
    "AllowAnyOrigin": true,
    "AllowAnyMethod": true,
    "AllowAnyHeader": true,
    "AllowedOrigins": [],
    "AllowedMethods": [],
    "AllowedHeaders": []
}
```

> [!NOTE]
> For migrations you can use a specific project to add to your solution, then configuring the assembly in _AppSettings:MigrationsAssembly_, otherwise leave it blank and the assembly containing the _Program.cs_ class will be used.

## 🗃️ Database

### Configuration

The library uses Entity Framework Core to manage the database.

The connection string is configured in the `AppSettings` section of the _appsettings.json_ file.

- Database Type: Set via `AppSettings:DatabaseType` (supported values: `sqlserver`, `azuresql`, `postgresql`, `mysql`, `sqlite`)

After setting the type of database you want to use, modify the corresponding connection string.

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

```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var minioOptions = builder.Services.ConfigureAndGet<MinioOptions>(builder.Configuration, nameof(MinioOptions)) ?? new MinioOptions();

        builder.Host.UseSerilogToStorageCloud((context, services, config)
            => config.ReadFrom.Configuration(context.Configuration), minioOptions);

        var appSettings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new();
        var jwtOptions = builder.Services.ConfigureAndGet<JwtOptions>(builder.Configuration, nameof(JwtOptions)) ?? new();
        var swaggerSettings = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new();
        var corsOptions = builder.Services.ConfigureAndGet<CorsOptions>(builder.Configuration, nameof(CorsOptions)) ?? new();

        builder.Services.AddRegisterDefaultServices<MinimalApiAuthDbContext>(builder.Configuration, appSettings, jwtOptions);
        //If you need to register services with a lifecycle other than Transient, do not modify this configuration,
        //but create one (or more) duplicates of this configuration, modifying it as needed.
        builder.Services.AddRegisterServices(options =>
        {
            options.Interfaces = [typeof(IAuthService)]; // Register your interfaces here, but do not remove the IAuthService service.
            options.StringEndsWith = "Service"; // This will register all services that end with "Service" in the assembly.
            options.Lifetime = ServiceLifetime.Transient; // This will register the services with a Transient lifetime.
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddDefaultSecurityOptions();
            // Here you can add additional authorization policies
        });

        var app = builder.Build();

		var activeModules = RegisterServicesExtensions.ReadFeatureFlags(appSettings);
		var appName = app.Environment.ApplicationName;
		
		await RegisterServicesExtensions.ConfigureDatabaseAsync(app.Services);
		
		// If behind a proxy, uncomment and configure the KnownProxies collection
		//app.UseForwardedHeaders(new()
		//{
		//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
		//    KnownProxies = { }
		//});
		
		app.UseHttpsRedirection();
		app.UseStatusCodePages();
		
		app.UseMiddleware<MinimalApiExceptionMiddleware>();
		if (swaggerSettings.IsEnabled)
		{
			if (swaggerSettings.AuthSettings.IsRequired)
			{
				app.UseMiddleware<SwaggerBasicAuthMiddleware>();
			}
		
			app.UseSwagger();
			app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{appName} v1"));
		}
		
		app.UseRouting();
		app.UseCors(corsOptions.PolicyName);
		
		app.UseAuthentication();
		app.UseAuthorization();
		
		app.UseMapEndpoints(activeModules);
        await app.RunAsync();
    }
}
```

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

## 📦 Packages

|Name|Type|Version|
|----|----|-------|
|[Identity.Module.API](https://www.nuget.org/packages/Identity.Module.API)|Main|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.API)](https://www.nuget.org/packages/Identity.Module.API)
|[Identity.Module.AccountManager](https://www.nuget.org/packages/Identity.Module.AccountManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.AccountManager)](https://www.nuget.org/packages/Identity.Module.AccountManager)|
|[Identity.Module.AuthManager](https://www.nuget.org/packages/Identity.Module.AuthManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.AuthManager)](https://www.nuget.org/packages/Identity.Module.AuthManager)|
|[Identity.Module.ClaimsManager](https://www.nuget.org/packages/Identity.Module.ClaimsManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.ClaimsManager)](https://www.nuget.org/packages/Identity.Module.ClaimsManager)|
|[Identity.Module.Core](https://www.nuget.org/packages/Identity.Module.Core)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.Core)](https://www.nuget.org/packages/Identity.Module.Core)|
|[Identity.Module.EmailManager](https://www.nuget.org/packages/Identity.Module.EmailManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.EmailManager)](https://www.nuget.org/packages/Identity.Module.EmailManager)|
|[Identity.Module.LicenseManager](https://www.nuget.org/packages/Identity.Module.LicenseManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.LicenseManager)](https://www.nuget.org/packages/Identity.Module.LicenseManager)|
|[Identity.Module.ModuleManager](https://www.nuget.org/packages/Identity.Module.ModuleManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.ModuleManager)](https://www.nuget.org/packages/Identity.Module.ModuleManager)|
|[Identity.Module.PolicyManager](https://www.nuget.org/packages/Identity.Module.PolicyManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.PolicyManager)](https://www.nuget.org/packages/Identity.Module.PolicyManager)|
|[Identity.Module.ProfileManager](https://www.nuget.org/packages/Identity.Module.ProfileManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.ProfileManager)](https://www.nuget.org/packages/Identity.Module.ProfileManager)|
|[Identity.Module.RolesManager](https://www.nuget.org/packages/Identity.Module.RolesManager)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.RolesManager)](https://www.nuget.org/packages/Identity.Module.RolesManager)|
|[Identity.Module.Shared](https://www.nuget.org/packages/Identity.Module.Shared)|Dependence|[![Nuget Package](https://badgen.net/nuget/v/Identity.Module.Shared)](https://www.nuget.org/packages/Identity.Module.Shared)|
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

- [ ] Replacing exceptions with implementation of operation results 
- [ ] Replacing the hosted service email sender using Coravel jobs
- [ ] Migrate solution to .NET 9
- [ ] Migrate FeatureFlagsOptions to Feature Management (package Microsoft.FeatureManagement)
- [ ] Migrate SwaggerSettings configuration to database
- [ ] Migrate SmtpOptions configuration to database
- [ ] Add endpoints for two-factor authentication and management
- [ ] Add endpoints for downloading and deleting personal data
- [ ] Code Review and Refactoring
- [ ] Migrate solution to .NET 10
- [ ] Add support for multi tenancy
- [ ] Align endpoints with the updated version of ASP.NET Core Identity
- [ ] Change the entity ID type from INT to GUID
- [ ] Make the ID entity type dynamic, so that it can accept both INT and GUID at runtime

## 🚀 Future implementations

- [ ] Add authentication support from third-party providers (e.g. Auth0, KeyCloak, GitHub, Azure)

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
