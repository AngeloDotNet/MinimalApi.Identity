using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Services;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.Shared.DependencyInjection;
using MinimalApi.Identity.Shared.Options;
using Serilog;

namespace IdentityManager.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var minioOptions = builder.Services.ConfigureAndGet<MinioOptions>(builder.Configuration, nameof(MinioOptions)) ?? new MinioOptions();

        builder.Host.UseSerilogToStorageCloud((context, services, config)
            => config.ReadFrom.Configuration(context.Configuration), minioOptions);

        var appSettings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new();
        var jwtOptions = builder.Services.ConfigureAndGet<JwtOptions>(builder.Configuration, nameof(JwtOptions)) ?? new();
        var swaggerSettings = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new();
        var corsOptions = builder.Services.ConfigureAndGet<CorsOptions>(builder.Configuration, nameof(CorsOptions)) ?? new();

        builder.Services.AddRegisterDefaultServices<MinimalApiAuthDbContext>(builder.Configuration, appSettings, jwtOptions);
        builder.Services.AddRegisterServices(options =>
        {
            options.Interfaces = [typeof(IAuthService)];
            options.StringEndsWith = "Service";
            options.Lifetime = ServiceLifetime.Transient;
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddDefaultSecurityOptions();
        });

        var app = builder.Build();

        var activeModules = RegisterServicesExtensions.ReadFeatureFlags(appSettings);
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
        if (swaggerSettings.IsEnabled)
        {
            if (swaggerSettings.AuthSettings.IsRequired)
            {
                app.UseMiddleware<SwaggerBasicAuthMiddleware>();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{appName} v1"));
        }

        app.UseRouting();
        app.UseCors(corsOptions.PolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMapEndpoints(activeModules);
        await app.RunAsync();
    }
}