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
using MinimalApi.Identity.API.Authorization.Handlers;
using MinimalApi.Identity.API.Configurations;
using MinimalApi.Identity.API.HostedServices;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.API.Validator;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Extensions;

public static class RegisterServicesExtensions
{
    public static IServiceCollection AddRegisterDefaultServices<TDbContext, TMigrations>(this IServiceCollection services, Action<DefaultServicesConfiguration> configure)
        where TDbContext : DbContext
        where TMigrations : class
    {
        var configuration = new DefaultServicesConfiguration(services);
        configure.Invoke(configuration);

        var apiValidationOptions = configuration.Configure.GetSection("ApiValidationOptions").Get<ApiValidationOptions>()
            ?? throw new ArgumentNullException("ApiValidationOptions", "api validation options not found");
        //var apiValidationOptions = ServicesExtensions.AddOptionValidate<ApiValidationOptions>(services, "ApiValidationOptions"); //TODO: code cleanup

        var hostedServiceOptions = configuration.Configure.GetSection("HostedServiceOptions").Get<HostedServiceOptions>()
            ?? throw new ArgumentNullException("HostedServiceOptions", "hosted service options not found");
        //var hostedServiceOptions = ServicesExtensions.AddOptionValidate<HostedServiceOptions>(services, "HostedServiceOptions"); //TODO: code cleanup

        var jwtOptions = configuration.Configure.GetSection("JwtOptions").Get<JwtOptions>()
            ?? throw new ArgumentNullException("JwtOptions", "JWT options not found");
        //var jwtOptions = ServicesExtensions.AddOptionValidate<JwtOptions>(services, "JwtOptions"); //TODO: code cleanup

        var identityOptions = configuration.Configure.GetSection("NetIdentityOptions").Get<NetIdentityOptions>()
            ?? throw new ArgumentNullException("NetIdentityOptions", "Identity options not found");
        //var identityOptions = ServicesExtensions.AddOptionValidate<NetIdentityOptions>(services, "NetIdentityOptions"); //TODO: code cleanup

        var smtpOptions = configuration.Configure.GetSection("SmtpOptions").Get<SmtpOptions>()
            ?? throw new ArgumentNullException("SmtpOptions", "SMTP options not found");
        //var smtpOptions = ServicesExtensions.AddOptionValidate<SmtpOptions>(services, "SmtpOptions"); //TODO: code cleanup

        var userOptions = configuration.Configure.GetSection("UsersOptions").Get<UsersOptions>()
            ?? throw new ArgumentNullException("UsersOptions", "Users options not found");
        //var userOptions = ServicesExtensions.AddOptionValidate<UsersOptions>(services, "UsersOptions"); //TODO: code cleanup

        services
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddSwaggerConfiguration()

            .AddMinimalApiDbContext<TDbContext>(configuration.DatabaseConnectionString, typeof(TMigrations).Assembly.FullName!)
            .AddMinimalApiIdentityServices<TDbContext, ApplicationUser>(jwtOptions)
            .AddMinimalApiIdentityOptionsServices(identityOptions)

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

        services.AddRegisterServices(options =>
        {
            options.Interfaces = [typeof(IAccountService)];
            options.StringEndsWith = "Service";
            options.Lifetime = ServiceLifetime.Transient;
        });

        services
            .AddSingleton<IHostedService, AuthorizationPolicyGeneration>()
            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>()
            .AddHostedService<AuthorizationPolicyUpdater>();

        return services;
    }

    public static void UseMapEndpoints(this WebApplication app) => app.MapEndpoints();

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

    internal static IServiceCollection AddMinimalApiDbContext<TDbContext>(this IServiceCollection services, string dbConnString,
        string migrationAssembly) where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>(options => options
            .UseSqlServer(dbConnString, opt =>
            {
                opt.MigrationsAssembly(migrationAssembly);
                opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            })
        );

        return services;
    }

    internal static IServiceCollection AddMinimalApiIdentityServices<TDbContext, TEntityUser>(this IServiceCollection services, JwtOptions jwtOptions)
        where TDbContext : DbContext
        where TEntityUser : class
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));

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
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    internal static IServiceCollection AddMinimalApiIdentityOptionsServices(this IServiceCollection services, NetIdentityOptions identityOptions)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = identityOptions.RequireUniqueEmail;

            options.Password = new PasswordOptions
            {
                RequireDigit = identityOptions.RequireDigit,
                RequiredLength = identityOptions.RequiredLength,
                RequireUppercase = identityOptions.RequireUppercase,
                RequireLowercase = identityOptions.RequireLowercase,
                RequireNonAlphanumeric = identityOptions.RequireNonAlphanumeric,
                RequiredUniqueChars = identityOptions.RequiredUniqueChars
            };

            options.SignIn.RequireConfirmedEmail = identityOptions.RequireConfirmedEmail;

            options.Lockout = new LockoutOptions
            {
                MaxFailedAccessAttempts = identityOptions.MaxFailedAccessAttempts,
                AllowedForNewUsers = identityOptions.AllowedForNewUsers,
                DefaultLockoutTimeSpan = identityOptions.DefaultLockoutTimeSpan
            };
        });

        return services;
    }
}