# 🗃️ Database

## Configuration

The library uses Entity Framework Core to manage the database.

The connection string is configured in the `ConnectionStrings` section of the _appsettings.json_ file.

- Database Type: Set via `ConnectionStrings:DatabaseType` (supported values: `sqlserver`)

After setting the type of database you want to use, modify the corresponding connection string.

## Migrations

> [!TIP]
> To update the database schema you need to create migrations, they will be applied automatically at the next application startup.

To create database migrations, follow these simple steps:

- Select `MinimalApi.Identity.Core` as the default project from the drop-down menu.

- In the `Package Manager Console`, run the following command: `Add-Migration InitialMigration -Project MinimalApi.Identity.Migrations.SQLServer`