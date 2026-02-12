using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Identity.Shared.Interceptor;
using MinimalApi.Identity.Shared.MinIO;
using MinimalApi.Identity.Shared.Options;
using Serilog;

namespace MinimalApi.Identity.Shared.DependencyInjection;

public static class SharedExtensions
{
    public static IServiceCollection AddInterceptor(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register interceptors as IInterceptor so GetServices<IInterceptor>() returns them
        // Documentation: https://dev.to/madusanka_bandara/mastering-database-interceptors-in-net-core-web-api-beginner-to-hero-18g8
        services.AddScoped<IInterceptor, LoggingInterceptor>();
        services.AddScoped<IInterceptor, PerformanceInterceptor>();
        services.AddScoped<IInterceptor, SecurityInterceptor>();
        services.AddScoped<IInterceptor, AuditSaveChangesInterceptor>();

        return services;
    }

    public static IHostBuilder UseSerilogToStorageCloud(this IHostBuilder hostBuilder, MinioOptions minioOptions)
    {
        return hostBuilder.UseSerilog((context, services, config)
            => config.ReadFrom.Configuration(context.Configuration).WriteTo.Sink(new MinioS3Sink(minioOptions)));
    }
}