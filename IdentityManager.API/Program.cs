using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Services;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.Shared.DependencyInjection;
using MinimalApi.Identity.Shared.Options;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;
using Serilog;

namespace IdentityManager.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var appSettings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new();
        var jwtOptions = builder.Services.ConfigureAndGet<JwtOptions>(builder.Configuration, nameof(JwtOptions)) ?? new();
        var swaggerSettings = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new();
        var corsOptions = builder.Services.ConfigureAndGet<CorsOptions>(builder.Configuration, nameof(CorsOptions)) ?? new();
        var minioOptions = builder.Services.ConfigureAndGet<MinioOptions>(builder.Configuration, nameof(MinioOptions)) ?? new MinioOptions();
        var smtpOptions = builder.Services.ConfigureAndGet<SmtpOptions>(builder.Configuration, nameof(SmtpOptions)) ?? new SmtpOptions();

        // Configure Serilog to use MinIO storage if access key or secret key is provided
        if (!string.IsNullOrEmpty(minioOptions.AccessKey) || !string.IsNullOrEmpty(minioOptions.SecretKey))
        {
            builder.Host.UseSerilogToStorageCloud((context, services, config)
                => config.ReadFrom.Configuration(context.Configuration), minioOptions);
        }

        var activeModules = RegisterServicesExtensions.ReadFeatureFlags(appSettings);

        builder.Services.AddRegisterDefaultServices<MinimalApiAuthDbContext>(options =>
        {
            options.Configuration = builder.Configuration;
            options.TypeDatabase = DatabaseType.SQLServer;                              // Alternatively: AzureSQL, PostgreSQL, MySQL, SQLite
            options.ErrorResponseFormat = ErrorResponseFormat.List;                     // Alternatively: Default
            options.ApplicationSettings = appSettings;
            options.JwtSettings = jwtOptions;
            options.CorsSettings = corsOptions;
            options.ActiveModules = activeModules;
            //options.SmtpSettings = smtpOptions;
            options.MigrationsAssembly = "MinimalApi.Identity.Migrations.SQLServer";    // Alternatively: typeof(Program).Assembly.FullName;

        });

        // If you need to register services with a lifecycle other than Transient, do not modify this configuration,
        // but create one (or more) duplicates of this configuration, modifying it as needed.
        builder.Services.AddRegisterServices(options =>
        {
            options.Interfaces = [typeof(IAuthService)];    // Register your interfaces here, but do not remove the IAuthService service.
            options.StringEndsWith = "Service";                     // This will register all services that end with "Service" in the assembly.
            options.Lifetime = ServiceLifetime.Transient;           // This will register the services with a Transient lifetime.
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddDefaultSecurityOptions();
            // Here you can add additional authorization policies
        });

        var app = builder.Build();
        var appName = app.Environment.ApplicationName;

        await RegisterServicesExtensions.ConfigureDatabaseAsync(app.Services);

        // If behind a proxy, uncomment and configure the KnownProxies collection
        //app.UseForwardedHeaders(new()
        //{
        //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        //    KnownProxies = { }
        //});

        app.UseHttpsRedirection();
        app.UseStatusCodePages();

        app.UseMiddleware<MinimalApiExceptionMiddleware>();

        // Enable Swagger only if it's enabled in settings
        if (swaggerSettings is { IsEnabled: true })
        {
            // Enable basic authentication for Swagger UI
            if (swaggerSettings.AuthSettings?.IsRequired == true)
            {
                app.UseMiddleware<SwaggerBasicAuthMiddleware>();
            }

            const string swaggerJson = "/swagger/v1/swagger.json";
            var swaggerTitle = string.IsNullOrWhiteSpace(appName) ? "API v1" : $"{appName} v1";

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(swaggerJson, swaggerTitle));
        }

        app.UseRouting();
        app.UseCors(corsOptions.PolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMapEndpoints(activeModules);
        await app.RunAsync();
    }
}