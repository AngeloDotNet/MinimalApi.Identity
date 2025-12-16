using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MinimalApi.Identity.API.Extensions;

public static class DatabasesExtensions
{
    public static DbContextOptionsBuilder AddSqlServerBuilder(this DbContextOptionsBuilder optionsBuilder, string sqlConnection, string migrationAssembly)
    {
        optionsBuilder.UseSqlServer(sqlConnection, opt =>
        {
            opt.MigrationsAssembly(migrationAssembly);
            opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

            TryApplyExceptionProcessor(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder AddAzureSqlBuilder(this DbContextOptionsBuilder optionsBuilder, string sqlConnection, string migrationAssembly)
    {
        optionsBuilder.UseAzureSql(sqlConnection, opt =>
        {
            opt.MigrationsAssembly(migrationAssembly);
            opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

            TryApplyExceptionProcessor(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder AddPostgreSqlBuilder(this DbContextOptionsBuilder optionsBuilder, string pgConnection, string migrationAssembly)
    {
        optionsBuilder.UseNpgsql(pgConnection, opt =>
        {
            opt.MigrationsAssembly(migrationAssembly);
            opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

            TryApplyExceptionProcessor(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder AddMySqlBuilder(this DbContextOptionsBuilder optionsBuilder, string mySqlConnection, string migrationAssembly)
    {
        optionsBuilder.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection), opt =>
        {
            opt.MigrationsAssembly(migrationAssembly);
            opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

            TryApplyExceptionProcessor(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder AddSqLiteBuilder(this DbContextOptionsBuilder optionsBuilder, string sqliteConnection, string migrationAssembly)
    {
        optionsBuilder.UseSqlite(sqliteConnection, opt =>
        {
            opt.MigrationsAssembly(migrationAssembly);
            opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

            TryApplyExceptionProcessor(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return optionsBuilder;
    }

    private static void TryApplyExceptionProcessor(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        try
        {
            // Search loaded assemblies for a public static method with signature:
            // static void UseExceptionProcessor(DbContextOptionsBuilder)
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var method = assemblies
                .SelectMany(a => GetTypesSafe(a))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .FirstOrDefault(m =>
                {
                    if (m.Name != "UseExceptionProcessor")
                    {
                        return false;
                    }

                    var parameters = m.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(DbContextOptionsBuilder);
                });

            method?.Invoke(null, [optionsBuilder]);
        }
        catch
        {
            // Swallow: provider package may not be referenced / available at runtime.
            // If you want diagnostics, log here using your logger.
        }
    }

    private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
        catch
        {
            return [];
        }
    }
}