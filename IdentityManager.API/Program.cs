using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;

namespace IdentityManager.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var jwtOptions = new JwtOptions();
        var featureFlagsOptions = new FeatureFlagsOptions();

        builder.Configuration.Bind(nameof(JwtOptions), jwtOptions);
        builder.Configuration.Bind(nameof(FeatureFlagsOptions), featureFlagsOptions);

        var databaseType = builder.Configuration.GetValue<string>("ConnectionStrings:DatabaseType") ?? "sqlserver";
        var migrationsAssembly = builder.Configuration.GetValue<string>("ConnectionStrings:MigrationsAssembly") ?? typeof(Program).Assembly.FullName!;
        var formatErrors = builder.Configuration.GetValue<ErrorResponseFormat>("ApplicationOptions:ErrorResponseFormat");

        builder.Services.AddRegisterDefaultServices<MinimalApiAuthDbContext>(builder.Configuration, options =>
        {
            options.DatabaseType = databaseType;
            options.MigrationsAssembly = migrationsAssembly;
            options.JwtOptions = jwtOptions;
            options.FeatureFlags = featureFlagsOptions;
            options.FormatErrorResponse = formatErrors;
        });

        //If you need to register services with a lifecycle other than Transient, do not modify this configuration,
        //but create one (or more) duplicates of this configuration, modifying it as needed.
        builder.Services.AddRegisterServices(options =>
        {
            options.Interfaces = [typeof(IAuthService)]; // Register your interfaces here, but do not remove the IAuthService service.
            options.StringEndsWith = "Service"; // This will register all services that end with "Service" in the assembly.
            options.Lifetime = ServiceLifetime.Transient; // This will register the services with a Transient lifetime.
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddDefaultSecurityOptions();

            // Here you can add additional authorization policies
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

        app.UseMapEndpoints(featureFlagsOptions);
        await app.RunAsync();
    }
}