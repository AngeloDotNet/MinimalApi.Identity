# Database

## Configuration

The library uses Entity Framework Core to manage the database.

The connection string is configured in the `ConnectionStrings` section of the _appsettings.json_ file.

- Database Type: Set via `ConnectionStrings:DatabaseType` (supported values: `sqlserver`)

After setting the type of database you want to use, modify the corresponding connection string.

## Migrations

> [!TIP]
> To update the database schema you need to create migrations, they will be applied automatically at the next application startup.

To create the database migrations, you can use the following command in the Package Manager Console:

```shell
Add-Migration InitialMigration -Project MinimalApi.Identity.Migrations
```

Select MinimalApi.Identity.Core as the default project from the drop-down menu, as shown in the image below.

<img width="796" height="146" alt="Screenshot 2025-08-03 214346" src="https://github.com/user-attachments/assets/c30283ec-2c2d-44d3-8cd4-406181f186b9" />
