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
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.Core.Authorization;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Endpoints;
using MinimalApi.Identity.PolicyManager.HostedServices;
using MinimalApi.Identity.PolicyManager.Services.Interfaces;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterDefaultServices<TDbContext, TMigrations>(this IServiceCollection services, Action<DefaultServicesConfiguration> configure)
        where TDbContext : DbContext
        where TMigrations : class
    {
        var configuration = new DefaultServicesConfiguration(services);
        configure.Invoke(configuration);

        var apiValidationOptions = configuration.Configure.GetSection("ApiValidationOptions").Get<ApiValidationOptions>()!;
        var hostedServiceOptions = configuration.Configure.GetSection("HostedServiceOptions").Get<HostedServiceOptions>()!;
        var jwtOptions = configuration.Configure.GetSection("JwtOptions").Get<JwtOptions>()!;
        var identityOptions = configuration.Configure.GetSection("NetIdentityOptions").Get<NetIdentityOptions>()!;
        var smtpOptions = configuration.Configure.GetSection("SmtpOptions").Get<SmtpOptions>()!;
        var userOptions = configuration.Configure.GetSection("UsersOptions").Get<UsersOptions>();

        if (apiValidationOptions is null)
        {
            throw new ArgumentNullException(nameof(configure), "ApiValidationOptions cannot be null.");
        }
        else if (hostedServiceOptions is null)
        {
            throw new ArgumentNullException(nameof(configure), "HostedServiceOptions cannot be null.");
        }
        else if (jwtOptions is null)
        {
            throw new ArgumentNullException(nameof(configure), "JwtOptions cannot be null.");
        }
        else if (identityOptions is null)
        {
            throw new ArgumentNullException(nameof(configure), "NetIdentityOptions cannot be null.");
        }
        else if (smtpOptions is null)
        {
            throw new ArgumentNullException(nameof(configure), "SmtpOptions cannot be null.");
        }
        else if (userOptions is null)
        {
            throw new ArgumentNullException(nameof(configure), "UsersOptions cannot be null.");
        }

        services
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddSwaggerConfiguration()
            .AddDatabaseContext<TDbContext>(options =>
            {
                options.Configure = configuration.Configure;
                options.MigrationsAssembly = typeof(TMigrations).Assembly.FullName!;
                options.DatabaseType = configuration.Configure.GetSection("ConnectionStrings").GetValue<string>("DatabaseType")!;
            })
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(options =>
            {
                options.JWTOptions = jwtOptions;
                options.IdentityOptions = identityOptions;
            })
            .ConfigureValidation(options => options.ErrorResponseFormat = configuration.FormatErrorResponse)
            .ConfigureFluentValidation<LoginValidator>()
            .Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            })
            .Configure<RouteOptions>(options => options.LowercaseUrls = true)
            .Configure<KestrelServerOptions>(configuration.Configure.GetSection("Kestrel"));

        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IAccountService), typeof(IAuthPolicyService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .PolicyManagerRegistrationService(); //Register PolicyManager package services

        services
            .AddSingleton<IHostedService, AuthorizationPolicyGeneration>()
            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>()
            .AddHostedService<AuthorizationPolicyUpdater>();

        return services;
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
                        Url = new Uri("https://angelo.aepserver.it/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
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
}