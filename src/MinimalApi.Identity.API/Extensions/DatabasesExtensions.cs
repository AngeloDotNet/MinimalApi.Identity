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
        });

        return optionsBuilder;
    }

    // Cached MethodInfo for UseExceptionProcessor (may be provided by a third-party package).
    // The lookup is done once and cached to avoid repeated expensive reflection scans.
    private static MethodInfo? cachedUseExceptionProcessor;
    private static int initialized;
    private static readonly object initLock = new();

    private static void TryApplyExceptionProcessor(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        try
        {
            EnsureInitialized();

            var method = cachedUseExceptionProcessor;

            // Invoke the discovered static extension method: UseExceptionProcessor(optionsBuilder)
            method?.Invoke(null, [optionsBuilder]);
        }
        catch
        {
            // Swallow: provider package may not be referenced / available at runtime.
            // If you want diagnostics, log here using your logger.
        }
    }

    // Initialize the cached MethodInfo exactly once in a thread-safe manner.
    private static void EnsureInitialized()
    {
        if (Volatile.Read(ref initialized) != 0)
        {
            return;
        }

        lock (initLock)
        {
            if (initialized != 0)
            {
                return;
            }

            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    var types = GetTypesSafe(assembly);

                    foreach (var t in types)
                    {
                        if (t is null)
                        {
                            continue;
                        }

                        MethodInfo[] methods;
                        try
                        {
                            methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
                        }
                        catch
                        {
                            continue;
                        }

                        foreach (var m in methods)
                        {
                            if (m.Name != "UseExceptionProcessor")
                            {
                                continue;
                            }

                            var parameters = m.GetParameters();

                            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(DbContextOptionsBuilder))
                            {
                                cachedUseExceptionProcessor = m;

                                // Mark initialized and return immediately
                                Volatile.Write(ref initialized, 1);
                                return;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore any reflection exceptions here
            }

            // Mark initialized even if method not found to avoid repeated scans.
            Volatile.Write(ref initialized, 1);
        }
    }

    private static IEnumerable<Type?> GetTypesSafe(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            var list = new List<Type?>();
            var types = ex.Types;

            if (types is not null)
            {
                for (var i = 0; i < types.Length; i++)
                {
                    var t = types[i];

                    if (t is not null)
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }
        catch
        {
            return [];
        }
    }

    //public static DbContextOptionsBuilder AddSqlServerBuilder(this DbContextOptionsBuilder optionsBuilder, string sqlConnection, string migrationAssembly)
    //{
    //    optionsBuilder.UseSqlServer(sqlConnection, opt =>
    //    {
    //        opt.MigrationsAssembly(migrationAssembly);
    //        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
    //        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    //        TryApplyExceptionProcessor(optionsBuilder);
    //    });

    //    return optionsBuilder;
    //}

    //public static DbContextOptionsBuilder AddAzureSqlBuilder(this DbContextOptionsBuilder optionsBuilder, string sqlConnection, string migrationAssembly)
    //{
    //    optionsBuilder.UseAzureSql(sqlConnection, opt =>
    //    {
    //        opt.MigrationsAssembly(migrationAssembly);
    //        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
    //        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    //        TryApplyExceptionProcessor(optionsBuilder);
    //    });

    //    return optionsBuilder;
    //}

    //public static DbContextOptionsBuilder AddPostgreSqlBuilder(this DbContextOptionsBuilder optionsBuilder, string pgConnection, string migrationAssembly)
    //{
    //    optionsBuilder.UseNpgsql(pgConnection, opt =>
    //    {
    //        opt.MigrationsAssembly(migrationAssembly);
    //        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
    //        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    //        TryApplyExceptionProcessor(optionsBuilder);
    //    });

    //    return optionsBuilder;
    //}

    //public static DbContextOptionsBuilder AddMySqlBuilder(this DbContextOptionsBuilder optionsBuilder, string mySqlConnection, string migrationAssembly)
    //{
    //    optionsBuilder.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection), opt =>
    //    {
    //        opt.MigrationsAssembly(migrationAssembly);
    //        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
    //        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    //        TryApplyExceptionProcessor(optionsBuilder);
    //    });

    //    return optionsBuilder;
    //}

    //public static DbContextOptionsBuilder AddSqLiteBuilder(this DbContextOptionsBuilder optionsBuilder, string sqliteConnection, string migrationAssembly)
    //{
    //    optionsBuilder.UseSqlite(sqliteConnection, opt =>
    //    {
    //        opt.MigrationsAssembly(migrationAssembly);
    //        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
    //        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    //        TryApplyExceptionProcessor(optionsBuilder);
    //    });

    //    return optionsBuilder;
    //}

    //private static void TryApplyExceptionProcessor(DbContextOptionsBuilder optionsBuilder)
    //{
    //    ArgumentNullException.ThrowIfNull(optionsBuilder);

    //    try
    //    {
    //        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    //        var method = assemblies.SelectMany(a => GetTypesSafe(a))
    //            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
    //            .FirstOrDefault(m =>
    //            {
    //                if (m.Name != "UseExceptionProcessor")
    //                {
    //                    return false;
    //                }

    //                var parameters = m.GetParameters();
    //                return parameters.Length == 1 && parameters[0].ParameterType == typeof(DbContextOptionsBuilder);
    //            });

    //        method?.Invoke(null, [optionsBuilder]);
    //    }
    //    catch
    //    {
    //        // Swallow: provider package may not be referenced / available at runtime.
    //        // If you want diagnostics, log here using your logger.
    //    }
    //}

    //private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
    //{
    //    try
    //    {
    //        return assembly.GetTypes();
    //    }
    //    catch (ReflectionTypeLoadException ex)
    //    {
    //        return ex.Types.Where(t => t is not null)!;
    //    }
    //    catch
    //    {
    //        return [];
    //    }
    //}
}