using System.Text;
using System.Text.Json.Serialization;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.AccountManager.DependencyInjection;
using MinimalApi.Identity.API.Endpoints;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.AuthManager.DependencyInjection;
using MinimalApi.Identity.Core.Converter;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.EmailManager.DependencyInjection;
using MinimalApi.Identity.LicenseManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.ProfileManager.DependencyInjection;
using MinimalApi.Identity.RolesManager.DependencyInjection;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterDefaultServices<TDbContext>(this IServiceCollection services, IConfiguration configuration,
        AppSettings appSettings, JwtOptions jwtOptions) where TDbContext : DbContext
    {
        var activeModules = new FeatureFlagsOptions
        {
            EnabledFeatureLicense = appSettings.EnabledFeatureLicense,
            EnabledFeatureModule = appSettings.EnabledFeatureModule
        };

        services
            .AddSingleton(TimeProvider.System)
            .AddHttpContextAccessor()
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
            })
            //.AddSwaggerConfiguration(activeModules)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(opt => opt.AddSwaggerGenOptions(activeModules))
            .AddDatabaseContext<TDbContext>(configuration, appSettings.DatabaseType, appSettings.MigrationsAssembly)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(jwtOptions)
            .AddRegisterFeatureFlags(activeModules)
            .AddProblemDetails()
            .AddCorsConfiguration()
            //.AddCorsConfiguration(configuration) // Use this line instead to configure CORS from appsettings
            .AddScoped<SignInManager<ApplicationUser>>();

        services
            .AccountManagerRegistrationService()
            .AuthManagerRegistrationService()
            //.ClaimsManagerRegistrationService() // Disabled for now (not implemented)
            .EmailManagerRegistrationService()
            .PolicyManagerRegistrationService()
            .ProfileManagerRegistrationService()
            .RolesManagerRegistrationService();

        switch (appSettings.ErrorResponseFormat)
        {
            case "Default":
                services.ConfigureValidation(options => options.ErrorResponseFormat = nameof(ErrorResponseFormat.Default));
                break;
            case "List":
                services.ConfigureValidation(options => options.ErrorResponseFormat = nameof(ErrorResponseFormat.List));
                break;
            default:
                services.ConfigureValidation(options => options.ErrorResponseFormat = nameof(ErrorResponseFormat.Default));
                break;
        }

        services
            .Configure<SmtpOptions>(options => configuration.GetSection(nameof(SmtpOptions)).Bind(options))
            .Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<KestrelServerOptions>(options => configuration.GetSection("Kestrel").Bind(options))
            .ConfigureFluentValidation<LoginValidator>();

        return services;
    }

    public static IServiceCollection AddRegisterFeatureFlags(this IServiceCollection services, FeatureFlagsOptions featureFlagsOptions)
    {
        if (featureFlagsOptions.EnabledFeatureLicense)
        {
            services.LicenseManagerRegistrationService();
        }

        // Disabled for now (not implemented)
        //if (featureFlagsOptions.EnabledFeatureModule)
        //{
        //    services.ModuleManagerRegistrationService();
        //}

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app, AppSettings appSettings)
    {
        var activeModules = new FeatureFlagsOptions
        {
            EnabledFeatureLicense = appSettings.EnabledFeatureLicense,
            EnabledFeatureModule = appSettings.EnabledFeatureModule
        };

        app.MapAuthEndpoints();
        app.MapClaimsEndpoints();

        //app.MapAccountEndpoints();
        //app.MapClaimsEndpoints();
        //app.MapPolicyEndpoints();
        //app.MapProfileEndpoints();
        //app.MapRolesEndpoints();

        //if (activeModules.EnabledFeatureLicense)
        //{
        //    app.MapLicenseEndpoints();
        //}

        if (activeModules.EnabledFeatureModule)
        {
            app.MapModuliEndpoints();
        }
    }

    //public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, FeatureFlagsOptions featureFlagsOptions)
    //{
    //    return services
    //        .AddEndpointsApiExplorer()
    //        .AddSwaggerGen(opt => opt.AddSwaggerGenOptions(featureFlagsOptions));
    //}

    public static IServiceCollection AddDatabaseContext<TDbContext>(this IServiceCollection services, IConfiguration configuration,
        string databaseType, string migrationAssembly) where TDbContext : DbContext
    {
        if (databaseType is null)
        {
            throw new InvalidOperationException("Database type is not configured.");
        }

        var dbType = databaseType.ToLowerInvariant();
        var sqlConnection = dbType switch
        {
            "sqlserver" => configuration.GetConnectionString("SQLServer"),
            "azuresql" => configuration.GetConnectionString("AzureSQL"),
            "postgresql" => configuration.GetConnectionString("PostgreSQL"),
            "mysql" => configuration.GetConnectionString("MySQL"),
            "sqlite" => configuration.GetConnectionString("SQLite"),
            _ => null
        } ?? throw new InvalidOperationException($"Connection string for '{databaseType}' is not configured.");

        Action<DbContextOptionsBuilder> optionsAction = dbType switch
        {
            "sqlserver" => options => options.UseSqlServer(sqlConnection, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                options.UseExceptionProcessor();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }),
            "azuresql" => options => options.UseAzureSql(sqlConnection, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                options.UseExceptionProcessor();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }),
            "postgresql" => options => options.UseNpgsql(sqlConnection, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                options.UseExceptionProcessor();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }),
            "mysql" => options => options.UseMySql(sqlConnection, ServerVersion.AutoDetect(sqlConnection), opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                options.UseExceptionProcessor();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }),
            "sqlite" => options => options.UseSqlite(sqlConnection, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                options.UseExceptionProcessor();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }),
            _ => _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
        };

        services.AddDbContext<TDbContext>(optionsAction);

        return services;
    }

    public static async Task ConfigureDatabaseAsync(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MinimalApiAuthDbContext>();

        // Try to migrate directly; EnsureCreated is not needed if using migrations.
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync().ConfigureAwait(false);

        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync().ConfigureAwait(false);
        }
        else
        {
            // Only ensure created if no migrations exist (e.g., for in-memory or initial setup)
            var canConnect = await dbContext.Database.CanConnectAsync().ConfigureAwait(false);

            if (!canConnect)
            {
                await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
            }
        }
    }

    public static AuthorizationOptions AddDefaultSecurityOptions(this AuthorizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var policyBuilder = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser();

        var policy = policyBuilder.Build();

        options.DefaultPolicy = policy;
        options.FallbackPolicy = policy;

        return options;
    }

    public static T? ConfigureAndGet<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(sectionName);

        var section = configuration.GetSection(sectionName);
        services.Configure<T>(section);

        var settings = Activator.CreateInstance<T>();
        section.Bind(settings);

        return settings;
    }

    internal static IServiceCollection AddMinimalApiIdentityServices<TDbContext, TEntityUser>(this IServiceCollection services, JwtOptions settings)
        where TDbContext : DbContext
        where TEntityUser : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(settings);

        services
            .AddIdentity<TEntityUser, ApplicationRole>()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();

        var securityKeyBytes = Encoding.UTF8.GetBytes(settings.SecurityKey);
        var signingKey = new SymmetricSecurityKey(securityKeyBytes);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    RequireExpirationTime = true,
                    ClockSkew = settings.ClockSkew
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = settings.RequireUniqueEmail;
            options.Password.RequireDigit = settings.RequireDigit;
            options.Password.RequiredLength = settings.RequiredLength;
            options.Password.RequireUppercase = settings.RequireUppercase;
            options.Password.RequireLowercase = settings.RequireLowercase;
            options.Password.RequireNonAlphanumeric = settings.RequireNonAlphanumeric;
            options.Password.RequiredUniqueChars = settings.RequiredUniqueChars;

            options.SignIn.RequireConfirmedEmail = settings.RequireConfirmedEmail;
            options.Lockout.MaxFailedAccessAttempts = settings.MaxFailedAccessAttempts;
            options.Lockout.AllowedForNewUsers = settings.AllowedForNewUsers;
            options.Lockout.DefaultLockoutTimeSpan = settings.DefaultLockoutTimeSpan;
        });

        return services;
    }

    internal static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        return services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
    }

    internal static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = new CorsOptions();
        configuration.GetSection("Cors").Bind(corsOptions);

        return services.AddCors(options => options.AddPolicy("cors", builder =>
        {
            if (corsOptions.AllowAnyOrigin)
            {
                builder.AllowAnyOrigin();
            }
            else if (corsOptions.AllowedOrigins.Length > 0)
            {
                builder.WithOrigins(corsOptions.AllowedOrigins);
            }

            if (corsOptions.AllowAnyMethod)
            {
                builder.AllowAnyMethod();
            }
            else if (corsOptions.AllowedMethods.Length > 0)
            {
                builder.WithMethods(corsOptions.AllowedMethods);
            }

            if (corsOptions.AllowAnyHeader)
            {
                builder.AllowAnyHeader();
            }
            else if (corsOptions.AllowedHeaders.Length > 0)
            {
                builder.WithHeaders(corsOptions.AllowedHeaders);
            }
        }));
    }
}