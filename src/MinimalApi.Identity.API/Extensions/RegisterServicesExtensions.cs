using System.Text;
using System.Text.Json.Serialization;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.AccountManager.DependencyInjection;
using MinimalApi.Identity.AccountManager.Endpoints;
using MinimalApi.Identity.API.Endpoints;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.AuthManager.DependencyInjection;
using MinimalApi.Identity.Core.Converter;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.EmailManager.DependencyInjection;
using MinimalApi.Identity.Licenses.DependencyInjection;
using MinimalApi.Identity.Licenses.Endpoints;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Endpoints;
using MinimalApi.Identity.ProfileManager.DependencyInjection;

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
            .AddSwaggerConfiguration(activeModules)
            .AddDatabaseContext<TDbContext>(configuration, appSettings.DatabaseType, appSettings.MigrationsAssembly)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(jwtOptions)
            .AddRegisterFeatureFlags(activeModules)
            .AddProblemDetails()
            .AddCorsConfiguration()
            .AddScoped<SignInManager<ApplicationUser>>();

        services
            .AccountManagerRegistrationService()
            .AuthManagerRegistrationService()
            .EmailManagerRegistrationService()
            .PolicyManagerRegistrationService()
            .ProfileManagerRegistrationService();

        //TODO: Missing services to register (Claims, Roles Manager Registration Services)

        var errorFormat = appSettings.ErrorResponseFormat;

        switch (errorFormat)
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
            services.LicenseRegistrationService();
            //TODO: Replace with new implementation: LicenseManagerRegistrationService();
        }

        if (featureFlagsOptions.EnabledFeatureModule)
        {
            //TODO: Replace with new implementation: ModuleManagerRegistrationService();
        }

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app, AppSettings appSettings)
    {
        var activeModules = new FeatureFlagsOptions
        {
            EnabledFeatureLicense = appSettings.EnabledFeatureLicense,
            EnabledFeatureModule = appSettings.EnabledFeatureModule
        };

        app.MapEndpoints();
        app.MapAccountEndpoints();
        app.MapPolicyEndpoints();
        app.MapProfileEndpoints();

        //TODO: Missing services to register (Claims, Roles Endpoints)

        if (activeModules.EnabledFeatureLicense)
        {
            app.MapLicenseEndpoints();
        }

        if (activeModules.EnabledFeatureModule)
        {
            //TODO: Replace with new implementation: app.MapModuleEndpoints();
        }
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, FeatureFlagsOptions featureFlagsOptions)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.AddOpenApiInfo();
                options.AddOpenApiSecuritySchemeRequirement();
                options.AddSwaggerDocumentFilters(featureFlagsOptions);
            });
    }

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
            //"azuresql" => configuration.GetConnectionString("AzureSQL"),
            //"postgresql" => configuration.GetConnectionString("PostgreSQL"),
            //"mysql" => configuration.GetConnectionString("MySQL"),
            //"sqlite" => configuration.GetConnectionString("SQLite"),
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
            //"azuresql" => options => options.UseAzureSql(sqlConnection, opt =>
            //{
            //    opt.MigrationsAssembly(migrationAssembly);
            //    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            //    opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

            //    options.UseExceptionProcessor();
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //}),
            //"postgresql" => options => options.UseNpgsql(sqlConnection, opt =>
            //{
            //    opt.MigrationsAssembly(migrationAssembly);
            //    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            //    opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            //}),
            //"mysql" => options => options.UseMySql(sqlConnection, ServerVersion.AutoDetect(sqlConnection), opt =>
            //{
            //    opt.MigrationsAssembly(migrationAssembly);
            //    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            //    opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            //}),
            //"sqlite" => options => options.UseSqlite(sqlConnection, opt =>
            //{
            //    opt.MigrationsAssembly(migrationAssembly);
            //    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            //    opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            //}),
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
}