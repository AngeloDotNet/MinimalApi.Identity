using System.Text;
using System.Text.Json;
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
using MinimalApi.Identity.API.Configurations;
using MinimalApi.Identity.API.Endpoints;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;
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
        Action<DefaultServicesConfiguration> configure) where TDbContext : DbContext
    {
        var settings = new DefaultServicesConfiguration(services);
        configure(settings);

        services
            .AddCorsConfiguration()
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddSwaggerConfiguration(settings.FeatureFlags)
            .AddDatabaseContext<TDbContext>(configuration, settings.DatabaseType, settings.MigrationsAssembly)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(settings.JwtOptions)
            .AddRegisterFeatureFlags(settings.FeatureFlags);

        services
            .AddScoped<SignInManager<ApplicationUser>>();

        services
            //.AccountManagerRegistrationService()
            .EmailManagerRegistrationService()
            .PolicyManagerRegistrationService()
            .ProfileManagerRegistrationService();

        services
            .Configure<JsonOptions>(options => options.ConfigureJsonOptions())
            .Configure<HostedServiceOptions>(options => configuration.GetSection(nameof(HostedServiceOptions)).Bind(options))
            .Configure<SmtpOptions>(options => configuration.GetSection(nameof(SmtpOptions)).Bind(options))
            .Configure<UsersOptions>(options => configuration.GetSection(nameof(UsersOptions)).Bind(options))
            .Configure<ValidationOptions>(options => configuration.GetSection(nameof(ValidationOptions)).Bind(options))

            .Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<KestrelServerOptions>(options => configuration.GetSection("Kestrel").Bind(options))

            .ConfigureValidation(options => options.ErrorResponseFormat = settings.FormatErrorResponse)
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

    public static void UseMapEndpoints(this WebApplication app, FeatureFlagsOptions featureFlagsOptions)
    {
        app.MapEndpoints();
        //app.MapAccountEndpoints();
        app.MapPolicyEndpoints()
            .MapProfileEndpoints();

        if (featureFlagsOptions.EnabledFeatureLicense)
        {
            app.MapLicenseEndpoints();
        }

        if (featureFlagsOptions.EnabledFeatureModule)
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

    public static ProgramOptions AddPublicOptions<T>(this ProgramOptions options, IConfiguration configuration) where T : class
    {
        var jwtOptions = new JwtOptions();
        var featureFlagsOptions = new FeatureFlagsOptions();

        configuration.Bind(nameof(JwtOptions), jwtOptions);
        configuration.Bind(nameof(FeatureFlagsOptions), featureFlagsOptions);

        var databaseType = configuration.GetValue<string>("ConnectionStrings:DatabaseType") ?? "sqlserver";
        var migrationsAssembly = configuration.GetValue<string>("ConnectionStrings:MigrationsAssembly") ?? typeof(T).Assembly.FullName!;
        var formatErrors = configuration.GetValue<ErrorResponseFormat>("ApplicationOptions:ErrorResponseFormat");

        options.JwtOptions = jwtOptions;
        options.FeatureFlagsOptions = featureFlagsOptions;
        options.DatabaseType = databaseType;
        options.MigrationsAssembly = migrationsAssembly;
        options.FormatErrors = formatErrors;

        return options;
    }

    internal static JsonOptions ConfigureJsonOptions(this JsonOptions jsonOptions)
    {
        ArgumentNullException.ThrowIfNull(jsonOptions);

        var options = new JsonOptions();

        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;

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