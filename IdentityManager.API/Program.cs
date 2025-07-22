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

        //...

        //Service has already been used within the library to register the necessary services, it is recommended
        //to use a different nomenclature. If you need to register services in the dependency injection container,
        //you can use this extension method, changing the lifetime property as needed.
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

        //...

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseStatusCodePages();

        //Use this middleware in your pipeline if you don't need to add new exceptions.
        app.UseMiddleware<MinimalApiExceptionMiddleware>();

        //If you need to add more exceptions, you need to add the ExtendedExceptionMiddleware middleware to your pipeline.
        //In the demo project, you can find a sample implementation (Middleware folder) to use to add the exceptions you need.
        //If you need it, remember to replace app.UseMiddleware<MinimalApiExceptionMiddleware>();
        //with app.UseMiddleware<ExtendedExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Environment.ApplicationName);
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