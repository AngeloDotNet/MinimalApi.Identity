using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Licenses.DependencyInjection;
using MinimalApi.Identity.Licenses.Endpoints;

namespace IdentityManager.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var migrationsAssembly = builder.Configuration.GetValue<string>("ApplicationOptions:MigrationsAssembly");

        if (string.IsNullOrEmpty(migrationsAssembly))
        {
            migrationsAssembly = typeof(Program).Assembly.FullName!;
        }

        builder.Services
            // If you need to register services with a lifecycle other than Transient, do not modify this configuration.
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IAuthService)]; // Register your interfaces here, but do not remove the IAuthService service.
                options.StringEndsWith = "Service"; // This will register all services that end with "Service" in the assembly.
                options.Lifetime = ServiceLifetime.Transient; // This will register the services with a Transient lifetime.
            })
            // Instead, create one (or more) duplicates like the one below, modifying it as needed.
            // The life cycle can take the values: Transient, Singleton and Scoped
            //.AddRegisterServices(options =>
            //{
            //    options.Interfaces = [typeof(YourServiceRepository)]; // Replace YourServiceRepository with your actual service interfaces.
            //    options.StringEndsWith = "Repository"; // This will register all services that end with "Repository" in the assembly.
            //    options.Lifetime = ServiceLifetime.Scoped; // This will register the services with a Scoped lifetime.
            //})
            .LicenseRegistrationService() //Register licensing services if this feature is needed, otherwise just comment out this line.
            .AddRegisterDefaultServices<MinimalApiAuthDbContext>(options =>
            {
                options.MigrationsAssembly = migrationsAssembly;
                options.FormatErrorResponse = ErrorResponseFormat.List; // or ErrorResponseFormat.Default
            }, configuration);

        builder.Services.AddAuthorization(options =>
        {
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

        app.UseMapEndpoints();
        app.MapLicenseEndpoints(); //Register licensing services if this feature is needed, otherwise just comment out this line.

        app.Run();
    }
}