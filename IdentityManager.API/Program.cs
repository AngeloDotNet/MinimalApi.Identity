using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;

namespace IdentityManager.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var programOptions = RegisterServicesExtensions.AddPublicOptions<Program>(new ProgramOptions(), builder.Configuration);

        builder.Services.AddRegisterDefaultServices<MinimalApiAuthDbContext>(builder.Configuration, options =>
        {
            options.DatabaseType = programOptions.DatabaseType;
            options.MigrationsAssembly = programOptions.MigrationsAssembly;
            options.JwtOptions = programOptions.JwtOptions;
            options.FeatureFlags = programOptions.FeatureFlagsOptions;
            options.FormatErrorResponse = programOptions.FormatErrors;
        });

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

        app.UseMapEndpoints(programOptions.FeatureFlagsOptions);
        await app.RunAsync();
    }
}