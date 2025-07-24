using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Licenses.DependencyInjection;
using MinimalApi.Identity.Licenses.Endpoints;

namespace IdentityManager.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options => options.AddPolicy("cors", builder
            => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        builder.Services.AddRegisterDefaultServices<MinimalApiAuthDbContext, Program>(options =>
        {
            options.Configure = builder.Configuration;
            options.FormatErrorResponse = ErrorResponseFormat.List; // or ErrorResponseFormat.Default
        });

        builder.Services.LicenseRegistrationService(); //Register licensing services if this feature is needed, otherwise just comment out this line.
        builder.Services.AddAuthorization(options =>
        {
            // Here you can add additional authorization policies
        });

        var app = builder.Build();
        //await ConfigureDatabaseAsync(app.Services);

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