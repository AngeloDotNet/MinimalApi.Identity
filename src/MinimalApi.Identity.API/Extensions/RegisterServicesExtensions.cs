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
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.Core.Authorization;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Endpoints;
using MinimalApi.Identity.PolicyManager.HostedServices;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterDefaultServices<TDbContext>(this IServiceCollection services,
        Action<DefaultServicesConfiguration> configure) where TDbContext : DbContext
    {
        var configuration = new DefaultServicesConfiguration(services);
        configure(configuration);

        var config = configuration.Configure;
        var hostedServiceOptions = config.GetSection(nameof(HostedServiceOptions)).Get<HostedServiceOptions>();
        var jwtOptions = config.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        var identityOptions = config.GetSection(nameof(NetIdentityOptions)).Get<NetIdentityOptions>();
        var smtpOptions = config.GetSection(nameof(SmtpOptions)).Get<SmtpOptions>();
        var userOptions = config.GetSection(nameof(UsersOptions)).Get<UsersOptions>();

        services
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddSwaggerConfiguration()
            .AddDatabaseContext<TDbContext>(options =>
            {
                options.Configure = configuration.Configure;
                options.MigrationsAssembly = configuration.MigrationsAssembly;
                options.DatabaseType = configuration.Configure.GetSection("ConnectionStrings").GetValue<string>("DatabaseType")!;
            })
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(options =>
            {
                options.JWTOptions = jwtOptions ?? new();
                options.IdentityOptions = identityOptions ?? new();
            })
            .AddOptionsConfiguration(configuration.Configure)
            .ConfigureValidation(options => options.ErrorResponseFormat = configuration.FormatErrorResponse)
            .ConfigureFluentValidation<LoginValidator>();

        services
            .AddTransient<AuthOptions>()
            .AddSingleton<IHostedService, AuthorizationPolicyGeneration>()
            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>()
            .AddHostedService<AuthorizationPolicyUpdater>();

        return services;
    }

    public static IServiceCollection RegisterServicesDefault(this IServiceCollection services)
    {
        return services
            .PolicyManagerRegistrationService() // Register PolicyManager package services
                                                //.EmailManagerRegistrationService() // Register EmailManager package services
            ;
    }

    public static void UseMapEndpoints(this WebApplication app)
    {
        app.MapEndpoints();
        app.MapPolicyEndpoints(); //Register PolicyManager package endpoints
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
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
            });
    }

    public static IServiceCollection AddDatabaseContext<TDbContext>(this IServiceCollection services, Action<DatabaseServiceConfiguration> configure)
        where TDbContext : DbContext
    {
        var configuration = new DatabaseServiceConfiguration(services);
        configure.Invoke(configuration);

        if (configuration.DatabaseType.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
        {
            services.AddDbContext<TDbContext>(options
                => options.UseSqlServer(configuration.Configure.GetConnectionString("SQLServer"), opt =>
                {
                    opt.MigrationsAssembly(configuration.MigrationsAssembly);
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
            options.User.RequireUniqueEmail = configuration.IdentityOptions.RequireUniqueEmail;
            options.Password = new PasswordOptions
            {
                RequireDigit = configuration.IdentityOptions.RequireDigit,
                RequiredLength = configuration.IdentityOptions.RequiredLength,
                RequireUppercase = configuration.IdentityOptions.RequireUppercase,
                RequireLowercase = configuration.IdentityOptions.RequireLowercase,
                RequireNonAlphanumeric = configuration.IdentityOptions.RequireNonAlphanumeric,
                RequiredUniqueChars = configuration.IdentityOptions.RequiredUniqueChars
            };

            options.SignIn.RequireConfirmedEmail = configuration.IdentityOptions.RequireConfirmedEmail;
            options.Lockout = new LockoutOptions
            {
                MaxFailedAccessAttempts = configuration.IdentityOptions.MaxFailedAccessAttempts,
                AllowedForNewUsers = configuration.IdentityOptions.AllowedForNewUsers,
                DefaultLockoutTimeSpan = configuration.IdentityOptions.DefaultLockoutTimeSpan
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
            .Configure<NetIdentityOptions>(configuration.GetSection("NetIdentityOptions"))
            .Configure<ValidationOptions>(configuration.GetSection("ValidationOptions"))
            .Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));

        return services;
    }
}