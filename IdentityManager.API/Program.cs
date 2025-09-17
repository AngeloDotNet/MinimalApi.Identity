using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;

namespace IdentityManager.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var appSettings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new();
        var jwtOptions = builder.Services.ConfigureAndGet<JwtOptions>(builder.Configuration, nameof(JwtOptions)) ?? new();

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
        await RegisterServicesExtensions.ConfigureDatabaseAsync(app.Services);

        app.UseHttpsRedirection();
        app.UseStatusCodePages();

        app.UseMiddleware<MinimalApiExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{app.Environment.ApplicationName} v1");
            });
        }

        app.UseRouting();
        app.UseCors("cors");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMapEndpoints(appSettings);
        await app.RunAsync();
    }
}