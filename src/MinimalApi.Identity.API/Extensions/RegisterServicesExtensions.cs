using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    //public static IServiceCollection AddRegisterDefaultServices<TDbContext>(this IServiceCollection services, IConfiguration configuration,
    //    AppSettings appSettings, JwtOptions jwtOptions) where TDbContext : DbContext
    public static IServiceCollection AddRegisterDefaultServices<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : DbContext
    {
        //var appSettings = new AppSettings();
        //configuration.GetSection(nameof(AppSettings)).Bind(appSettings);

        //var jwtOptions = new JwtOptions();
        //configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);

        var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>() ?? new AppSettings();
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>() ?? new JwtOptions();

        var activeModules = ReadFeatureFlags(appSettings);

        services
            .AddSingleton(TimeProvider.System)
            .AddHttpContextAccessor()
            .AddInterceptor() // Add Logging, Security, Performance and EF Core (for auditing) interceptors
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
            })
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(opt => opt.AddSwaggerGenOptions(activeModules))
            //.AddDatabaseContext<TDbContext>(configuration, appSettings)
            .AddDatabaseContext<TDbContext>(configuration)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(jwtOptions);
        //.AddRegisterPackagedServices(activeModules)
        //.AddProblemDetails()
        //.AddCorsConfiguration(configuration)
        //.AddScoped<SignInManager<ApplicationUser>>();

        //switch (appSettings.ErrorResponseFormat)
        //{
        //    case "Default":
        //        services.ConfigureValidation(options => options.ErrorResponseFormat = nameof(ErrorResponseFormat.Default));
        //        break;
        //    case "List":
        //        services.ConfigureValidation(options => options.ErrorResponseFormat = nameof(ErrorResponseFormat.List));
        //        break;
        //    default:
        //        services.ConfigureValidation(options => options.ErrorResponseFormat = nameof(ErrorResponseFormat.Default));
        //        break;
        //}

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

        services.AddProblemDetails()
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

    //public static IServiceCollection AddRegisterPackagedServices(this IServiceCollection services, FeatureFlagsOptions activeModules)
    //{
    //    services
    //        .AccountManagerRegistrationService()
    //        .AuthManagerRegistrationService()
    //        .ClaimsManagerRegistrationService()
    //        .EmailManagerRegistrationService()
    //        .PolicyManagerRegistrationService()
    //        .ProfileManagerRegistrationService()
    //        .RolesManagerRegistrationService();

    //    if (activeModules.EnabledFeatureLicense)
    //    {
    //        services.LicenseManagerRegistrationService();
    //    }

    //    if (activeModules.EnabledFeatureModule)
    //    {
    //        services.ModuleManagerRegistrationService();
    //    }

    //    return services;
    //}

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

    //public static IServiceCollection AddDatabaseContext<TDbContext>(this IServiceCollection services, IConfiguration configuration, AppSettings appSettings) where TDbContext : DbContext
    public static IServiceCollection AddDatabaseContext<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : DbContext
    {
        //ArgumentNullException.ThrowIfNull(appSettings);
        var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>() ?? new AppSettings();

        //var databaseType = appSettings.DatabaseType.ToLowerInvariant();
        //var migrationsAssembly = appSettings.MigrationsAssembly ?? throw new InvalidOperationException("Migrations assembly is not configured.");

        //var sqlConnection = GetDatabaseConnectionString(configuration, databaseType);
        //var optionsAction = GetDatabaseOptionsBuilder(databaseType, sqlConnection, migrationsAssembly);

        var databaseType = appSettings.DatabaseType ?? throw new InvalidOperationException("Database type is not configured.");
        var migrationsAssembly = appSettings.MigrationsAssembly ?? throw new InvalidOperationException("Migrations assembly is not configured.");

        databaseType = databaseType.ToLowerInvariant();

        var sqlConnection = GetDatabaseConnectionString(configuration, databaseType);
        var optionsAction = GetDatabaseOptionsBuilder(databaseType, sqlConnection, migrationsAssembly);

        services.AddDbContext<TDbContext>((serviceProvider, options) =>
        {
            // Apply the base options configured by optionsAction
            optionsAction(options);

            // Resolve EF Core interceptors registered in DI and add them dynamically.
            var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();

            if (interceptors.Length != 0)
            {
                options.AddInterceptors(interceptors);
            }

            // Helpful for debugging — optional in production
            options.EnableSensitiveDataLogging();
            options.LogTo(Console.WriteLine, LogLevel.Information);

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }

    //public static async Task ConfigureDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    //{
    //    ArgumentNullException.ThrowIfNull(serviceProvider);

    //    await using var scope = serviceProvider.CreateAsyncScope();

    //    var dbContext = scope.ServiceProvider.GetRequiredService<MinimalApiAuthDbContext>();
    //    var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);

    //    if (!canConnect)
    //    {
    //        await dbContext.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
    //        return;
    //    }

    //    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false);

    //    using var enumerator = pendingMigrations.GetEnumerator();

    //    if (enumerator.MoveNext())
    //    {
    //        await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    //    }
    //}

    public static async Task ConfigureDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MinimalApiAuthDbContext>();

        if (!await dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false))
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false);

        // Fast-path when the returned collection exposes Count to avoid enumerator allocation.
        var hasPending = pendingMigrations is ICollection<string> coll ? coll.Count > 0 : pendingMigrations.GetEnumerator().MoveNext();

        if (hasPending)
        {
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public static AuthorizationOptions AddDefaultSecurityOptions(this AuthorizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();

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

    internal static string GetDatabaseConnectionString(IConfiguration configuration, string databaseType)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(databaseType);

        return databaseType.ToLowerInvariant() switch
        {
            "sqlserver" => configuration.GetConnectionString("SQLServer") ?? string.Empty,
            "azuresql" => configuration.GetConnectionString("AzureSQL") ?? string.Empty,
            "postgresql" => configuration.GetConnectionString("PostgreSQL") ?? string.Empty,
            "mysql" => configuration.GetConnectionString("MySQL") ?? string.Empty,
            "sqlite" => configuration.GetConnectionString("SQLite") ?? string.Empty,
            _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
        };
    }

    internal static Action<DbContextOptionsBuilder> GetDatabaseOptionsBuilder(string databaseType, string sqlConnection, string migrationsAssembly)
    {
        ArgumentNullException.ThrowIfNull(databaseType);
        ArgumentNullException.ThrowIfNull(sqlConnection);
        ArgumentNullException.ThrowIfNull(migrationsAssembly);

        return databaseType.ToLowerInvariant() switch
        {
            "sqlserver" => options => options.AddSqlServerBuilder(sqlConnection, migrationsAssembly),
            "azuresql" => options => options.AddAzureSqlBuilder(sqlConnection, migrationsAssembly),
            "postgresql" => options => options.AddPostgreSqlBuilder(sqlConnection, migrationsAssembly),
            "mysql" => options => options.AddMySqlBuilder(sqlConnection, migrationsAssembly),
            "sqlite" => options => options.AddSqLiteBuilder(sqlConnection, migrationsAssembly),
            _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
        };
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