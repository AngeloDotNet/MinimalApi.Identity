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
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Configurations;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Swagger;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.Core.Authorization;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Licenses.DependencyInjection;
using MinimalApi.Identity.Licenses.Endpoints;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Endpoints;
using MinimalApi.Identity.PolicyManager.HostedServices;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterDefaultServices<TDbContext>(this IServiceCollection services,
        Action<DefaultServicesConfiguration> configure, IConfiguration configuration) where TDbContext : DbContext
    {
        var settings = new DefaultServicesConfiguration(services);
        configure(settings);

        var hostedServiceOptions = configuration.GetSection(nameof(HostedServiceOptions)).Get<HostedServiceOptions>();
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        var smtpOptions = configuration.GetSection(nameof(SmtpOptions)).Get<SmtpOptions>();
        var userOptions = configuration.GetSection(nameof(UsersOptions)).Get<UsersOptions>();

        services
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddSwaggerConfiguration(settings.FeatureFlags)
            .AddDatabaseContext<TDbContext>(options =>
            {
                options.MigrationsAssembly = settings.MigrationsAssembly;
                options.DatabaseType = configuration.GetSection("ConnectionStrings").GetValue<string>("DatabaseType")!;
            }, configuration)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(options =>
            {
                options.JWTOptions = jwtOptions ?? new();
            })
            .AddOptionsConfiguration(configuration)
            .ConfigureValidation(options => options.ErrorResponseFormat = settings.FormatErrorResponse)
            .ConfigureFluentValidation<LoginValidator>();

        services
            .AddSingleton<IHostedService, AuthorizationPolicyGeneration>()
            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>()
            .AddHostedService<AuthorizationPolicyUpdater>();

        return services;
    }

    public static IServiceCollection AddModulesRegistrations(this IServiceCollection services, FeatureFlagsOptions featureFlagsOptions)
    {
        services.PolicyManagerRegistrationService(); // Register PolicyManager package services
                                                     //.AccountManagerRegistrationService() // Register AccountManager package services
                                                     //.EmailManagerRegistrationService() // Register EmailManager package services

        if (featureFlagsOptions.EnabledFeatureLicense)
        {
            services.LicenseRegistrationService();
        }

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app, FeatureFlagsOptions featureFlagsOptions)
    {
        app.MapEndpoints();
        //app.MapAccountEndpoints(); //Register AccountManager package endpoints
        //app.MapEmailEndpoints(); //Register EmailManager package endpoints
        app.MapPolicyEndpoints(); //Register PolicyManager package endpoints

        if (featureFlagsOptions.EnabledFeatureLicense)
        {
            app.MapLicenseEndpoints();
        }
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, FeatureFlagsOptions featureFlagsOptions)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                var openApiInfo = new OpenApiInfo
                {
                    Title = "Minimal API Identity",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Angelo Pirola",
                        Email = "angelo@aepserver.it",
                        Url = new Uri(ConstantsConfiguration.WebSiteDev)
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License MIT",
                        Url = new Uri(ConstantsConfiguration.LicenseMIT)
                    },
                };
                options.SwaggerDoc("v1", openApiInfo);

                var securityScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insert the Bearer Token",
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                };
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                };
                options.AddSecurityRequirement(securityRequirement);

                // Register SwaggerModule DocumentFilter to conditionally remove inactive endpoints
                options.DocumentFilter<SwaggerModulesDocumentFilter>(featureFlagsOptions);
            });
    }

    public static IServiceCollection AddDatabaseContext<TDbContext>(this IServiceCollection services, Action<DatabaseServiceConfiguration> configure,
        IConfiguration configuration) where TDbContext : DbContext
    {
        var settings = new DatabaseServiceConfiguration(services);
        configure.Invoke(settings);

        if (settings.DatabaseType.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
        {
            var sqlConnection = configuration.GetConnectionString("SQLServer");

            services.AddDbContext<TDbContext>(options
                => options.UseSqlServer(sqlConnection, opt =>
                {
                    opt.MigrationsAssembly(settings.MigrationsAssembly);
                    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                    opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }));
        }

        return services;
    }

    public static async Task ConfigureDatabaseAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MinimalApiAuthDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    internal static IServiceCollection AddMinimalApiIdentityServices<TDbContext, TEntityUser>(this IServiceCollection services,
        Action<IdentityServicesConfiguration> configure) where TDbContext : DbContext where TEntityUser : class
    {
        var configuration = new IdentityServicesConfiguration(services);
        configure.Invoke(configuration);

        services
            .AddIdentity<TEntityUser, ApplicationRole>()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration.JWTOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = configuration.JWTOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.JWTOptions.SecurityKey)),
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = configuration.JWTOptions.RequireUniqueEmail;
            options.Password = new PasswordOptions
            {
                RequireDigit = configuration.JWTOptions.RequireDigit,
                RequiredLength = configuration.JWTOptions.RequiredLength,
                RequireUppercase = configuration.JWTOptions.RequireUppercase,
                RequireLowercase = configuration.JWTOptions.RequireLowercase,
                RequireNonAlphanumeric = configuration.JWTOptions.RequireNonAlphanumeric,
                RequiredUniqueChars = configuration.JWTOptions.RequiredUniqueChars
            };

            options.SignIn.RequireConfirmedEmail = configuration.JWTOptions.RequireConfirmedEmail;
            options.Lockout = new LockoutOptions
            {
                MaxFailedAccessAttempts = configuration.JWTOptions.MaxFailedAccessAttempts,
                AllowedForNewUsers = configuration.JWTOptions.AllowedForNewUsers,
                DefaultLockoutTimeSpan = configuration.JWTOptions.DefaultLockoutTimeSpan
            };
        });

        return services;
    }

    internal static IServiceCollection AddOptionsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            })
            .Configure<JwtOptions>(configuration.GetSection("JwtOptions"))
            .Configure<ValidationOptions>(configuration.GetSection("ValidationOptions"))
            .Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));

        return services;
    }
}