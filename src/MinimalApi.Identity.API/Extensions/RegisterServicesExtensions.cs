using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.AccountManager.DependencyInjection;
using MinimalApi.Identity.AccountManager.Endpoints;
using MinimalApi.Identity.API.Endpoints;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.AuthManager.DependencyInjection;
using MinimalApi.Identity.ClaimsManager.DependencyInjection;
using MinimalApi.Identity.ClaimsManager.Endpoints;
using MinimalApi.Identity.Core.Converter;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.EmailManager.DependencyInjection;
using MinimalApi.Identity.LicenseManager.DependencyInjection;
using MinimalApi.Identity.LicenseManager.Endpoints;
using MinimalApi.Identity.ModuleManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Endpoints;
using MinimalApi.Identity.ProfileManager.DependencyInjection;
using MinimalApi.Identity.RolesManager.DependencyInjection;
using MinimalApi.Identity.RolesManager.Endpoints;
using MinimalApi.Identity.Shared.DependencyInjection;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;
using optionsCors = MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterDefaultServices<TDbContext>(this IServiceCollection services, IConfiguration configuration,
        AppSettings appSettings, JwtOptions jwtOptions) where TDbContext : DbContext
    {
        var activeModules = ReadFeatureFlags(appSettings);

        services
            .AddSingleton(TimeProvider.System)
            .AddHttpContextAccessor()
            .AddInterceptor() // Add Logging, Performance and EF Core (for auditing) interceptors
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
            })
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(opt => opt.AddSwaggerGenOptions(activeModules))
            .AddDatabaseContext<TDbContext>(configuration, appSettings.DatabaseType, appSettings.MigrationsAssembly)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(jwtOptions)
            .AddRegisterPackagedServices(activeModules)
            .AddProblemDetails()
            .AddCorsConfiguration(configuration)
            .AddScoped<SignInManager<ApplicationUser>>();

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

    public static IServiceCollection AddRegisterPackagedServices(this IServiceCollection services, FeatureFlagsOptions activeModules)
    {
        services
            .AccountManagerRegistrationService()
            .AuthManagerRegistrationService()
            .ClaimsManagerRegistrationService()
            .EmailManagerRegistrationService()
            .PolicyManagerRegistrationService()
            .ProfileManagerRegistrationService()
            .RolesManagerRegistrationService();

        if (activeModules.EnabledFeatureLicense)
        {
            services.LicenseManagerRegistrationService();
        }

        if (activeModules.EnabledFeatureModule)
        {
            services.ModuleManagerRegistrationService();
        }

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app, FeatureFlagsOptions activeModules)
    {
        app.MapEndpointsFromAssemblyContaining<AuthEndpoints>();
        app.MapEndpointsFromAssemblyContaining<AccountEndpoints>();
        app.MapEndpointsFromAssemblyContaining<ClaimsEndpoints>();
        app.MapEndpointsFromAssemblyContaining<PolicyEndpoints>();
        app.MapEndpointsFromAssemblyContaining<ProfilesEndpoints>();
        app.MapEndpointsFromAssemblyContaining<RolesEndpoints>();

        if (activeModules.EnabledFeatureLicense)
        {
            app.MapEndpointsFromAssemblyContaining<LicenseEndpoints>();
        }

        if (activeModules.EnabledFeatureModule)
        {
            app.MapEndpointsFromAssemblyContaining<ModuliEndpoints>();
        }
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
            "azuresql" => configuration.GetConnectionString("AzureSQL"),
            "postgresql" => configuration.GetConnectionString("PostgreSQL"),
            "mysql" => configuration.GetConnectionString("MySQL"),
            "sqlite" => configuration.GetConnectionString("SQLite"),
            _ => null
        } ?? throw new InvalidOperationException($"Connection string for '{databaseType.ToUpperInvariant()}' is not configured.");

        Action<DbContextOptionsBuilder> optionsAction = dbType switch
        {
            "sqlserver" => options => options.AddSqlServerBuilder(sqlConnection, migrationAssembly),
            "azuresql" => options => options.AddAzureSqlBuilder(sqlConnection, migrationAssembly),
            "postgresql" => options => options.AddPostgreSqlBuilder(sqlConnection, migrationAssembly),
            "mysql" => options => options.AddMySqlBuilder(sqlConnection, migrationAssembly),
            "sqlite" => options => options.AddSqLiteBuilder(sqlConnection, migrationAssembly),
            _ => _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
        };

        services.AddDbContext<TDbContext>(optionsAction);

        return services;
    }

    public static async Task ConfigureDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await using var scope = serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MinimalApiAuthDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);

        if (!canConnect)
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false);

        using var enumerator = pendingMigrations.GetEnumerator();

        if (enumerator.MoveNext())
        {
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public static AuthorizationOptions AddDefaultSecurityOptions(this AuthorizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

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

    public static FeatureFlagsOptions ReadFeatureFlags(AppSettings appSettings)
    {
        var enabledFeatureLicense = appSettings.EnabledFeatureLicense;
        var enabledFeatureModule = appSettings.EnabledFeatureModule;

        var activeModules = new FeatureFlagsOptions
        {
            EnabledFeatureLicense = enabledFeatureLicense,
            EnabledFeatureModule = enabledFeatureModule
        };

        return activeModules;
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

    internal static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var corsOptions = configuration.GetSection(nameof(optionsCors.CorsOptions)).Get<optionsCors.CorsOptions>() ?? new optionsCors.CorsOptions();

        var policyName = corsOptions.PolicyName ?? string.Empty;
        var allowAnyOrigin = corsOptions.AllowAnyOrigin;
        var allowedOrigins = corsOptions.AllowedOrigins;
        var allowAnyHeader = corsOptions.AllowAnyHeader;
        var allowedHeaders = corsOptions.AllowedHeaders;
        var allowAnyMethod = corsOptions.AllowAnyMethod;
        var allowedMethods = corsOptions.AllowedMethods;

        services.AddCors(options => options.AddPolicy(policyName, builder =>
        {
            if (allowAnyOrigin)
            {
                builder.AllowAnyOrigin();
            }
            else if (allowedOrigins is { Length: > 0 })
            {
                builder.WithOrigins(allowedOrigins);
                builder.AllowCredentials();
            }

            if (allowAnyHeader)
            {
                builder.AllowAnyHeader();
            }
            else if (allowedHeaders is { Length: > 0 })
            {
                builder.WithHeaders(allowedHeaders);
            }

            if (allowAnyMethod)
            {
                builder.AllowAnyMethod();
            }
            else if (allowedMethods is { Length: > 0 })
            {
                builder.WithMethods(allowedMethods);
            }
        }));

        return services;
    }
}