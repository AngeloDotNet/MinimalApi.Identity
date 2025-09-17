using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            .AddCorsConfiguration()
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddSwaggerConfiguration(activeModules)
            .AddDatabaseContext<TDbContext>(configuration, appSettings.DatabaseType, appSettings.MigrationsAssembly)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(jwtOptions)
            .AddRegisterFeatureFlags(activeModules);

        services
            .AddScoped<SignInManager<ApplicationUser>>();

        services
            .AccountManagerRegistrationService()
            .AuthManagerRegistrationService()
            .EmailManagerRegistrationService()
            .PolicyManagerRegistrationService()
            .ProfileManagerRegistrationService();

        //TODO: Missing services to register
        //.ClaimsManagerRegistrationService()
        //.RolesManagerRegistrationService()

        var errorFormat = appSettings.ErrorResponseFormat;

        switch (errorFormat)
        {
            case "Default":
                services.ConfigureValidation(options => options.ErrorResponseFormat = ErrorResponseFormat.Default);
                break;
            case "List":
                services.ConfigureValidation(options => options.ErrorResponseFormat = ErrorResponseFormat.List);
                break;
            default:
                // Optionally, log a warning here about the unexpected value.
                services.ConfigureValidation(options => options.ErrorResponseFormat = ErrorResponseFormat.Default);
                break;
        }

        services
            .Configure<JsonOptions>(options => options.ConfigureJsonOptions())
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
            //services.LicenseManagerRegistrationService();
        }

        if (featureFlagsOptions.EnabledFeatureModule)
        {
            //services.ModuleManagerRegistrationService();
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

        //TODO: Missing services to register
        //app.MapClaimsEndpoints();
        //app.MapRolesEndpoints();

        if (activeModules.EnabledFeatureLicense)
        {
            app.MapLicenseEndpoints();
        }

        if (activeModules.EnabledFeatureModule)
        {
            //app.MapModuleEndpoints();
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
        if (databaseType is not null)
        {
            if (databaseType.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
            {
                var sqlConnection = configuration.GetConnectionString("SQLServer") ?? "SQLServer connection string is not configured.";

                services.AddDbContext<TDbContext>(options =>
                    options.UseSqlServer(sqlConnection, opt =>
                    {
                        opt.MigrationsAssembly(migrationAssembly);
                        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }));
            }

            //if (databaseType.Equals("postgresql", StringComparison.InvariantCultureIgnoreCase))
            //{
            //var sqlConnection = configuration.GetConnectionString("PostgreSQL");
            //services.AddDbContext<TDbContext>(options => options.UseNpgsql(sqlConnection, opt =>
            //{
            //    opt.MigrationsAssembly(migrationAssembly);
            //    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            //    opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            //}));
            //}

            //if (databaseType.Equals("mysql", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    var sqlConnection = configuration.GetConnectionString("MySQL");
            //    services.AddDbContext<TDbContext>(options => options.UseMySql(sqlConnection, ServerVersion.AutoDetect(sqlConnection), opt =>
            //    {
            //        opt.MigrationsAssembly(migrationAssembly);
            //        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            //        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            //    }));
            //}

            //if (databaseType.Equals("sqlite", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    var sqlConnection = configuration.GetConnectionString("SQLite");
            //    services.AddDbContext<TDbContext>(options => options.UseSqlite(sqlConnection, opt =>
            //    {
            //        opt.MigrationsAssembly(migrationAssembly);
            //        opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            //        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            //    }));
            //}
        }

        return services;
    }

    public static async Task ConfigureDatabaseAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MinimalApiAuthDbContext>();

        var databaseExist = await dbContext.Database.CanConnectAsync();

        if (!databaseExist)
        {
            await dbContext.Database.EnsureCreatedAsync();
        }

        var migrationsExist = (await dbContext.Database.GetPendingMigrationsAsync()).Any();

        if (migrationsExist)
        {
            await dbContext.Database.MigrateAsync();
        }
    }

    public static AuthorizationOptions AddDefaultSecurityOptions(this AuthorizationOptions options)
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

        return options;
    }

    public static T? ConfigureAndGet<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class
    {
        var section = configuration.GetSection(sectionName);
        var settings = section.Get<T>();
        services.Configure<T>(section);

        return settings;
    }

    internal static JsonOptions ConfigureJsonOptions(this JsonOptions jsonOptions)
    {
        ArgumentNullException.ThrowIfNull(jsonOptions);

        var options = new JsonOptions();

        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());

        return options;
    }

    internal static IServiceCollection AddMinimalApiIdentityServices<TDbContext, TEntityUser>(this IServiceCollection services,
        JwtOptions settings) where TDbContext : DbContext where TEntityUser : class
    {
        services
            .AddIdentity<TEntityUser, ApplicationRole>()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecurityKey)),
                    RequireExpirationTime = true,
                    ClockSkew = settings.ClockSkew
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = settings.RequireUniqueEmail;
            options.Password = new PasswordOptions
            {
                RequireDigit = settings.RequireDigit,
                RequiredLength = settings.RequiredLength,
                RequireUppercase = settings.RequireUppercase,
                RequireLowercase = settings.RequireLowercase,
                RequireNonAlphanumeric = settings.RequireNonAlphanumeric,
                RequiredUniqueChars = settings.RequiredUniqueChars
            };

            options.SignIn.RequireConfirmedEmail = settings.RequireConfirmedEmail;
            options.Lockout = new LockoutOptions
            {
                MaxFailedAccessAttempts = settings.MaxFailedAccessAttempts,
                AllowedForNewUsers = settings.AllowedForNewUsers,
                DefaultLockoutTimeSpan = settings.DefaultLockoutTimeSpan
            };
        });

        return services;
    }

    internal static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        return services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
    }
}