# 🗃️ Database

## Configuration

The library uses Entity Framework Core to manage the database.

The connection string is configured in the `ConnectionStrings` section of the _appsettings.json_ file.

- Database Type: Set via `ConnectionStrings:DatabaseType` (supported values: `sqlserver`)

After setting the type of database you want to use, modify the corresponding connection string.

## Migrations

> [!TIP]
> To update the database schema you need to create migrations, they will be applied automatically at the next application startup.

To create database migrations select `MinimalApi.Identity.Core` as the default project from the drop-down menu in the `Package Manager Console`
and run the command: `Add-Migration MIGRATION-NAME`

> [!NOTE]
> if you use a separate project for migrations (It is recommended to add a reference in the project name to the database used, in this case it is SQL Server), 
> make sure to set the `-Project` parameter to the name of that project.

Example: `Add-Migration InitialMigration -Project MinimalApi.Identity.Migrations.SQLServer`
