using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddScoped<IInterceptor, LoggingInterceptor>();
        services.AddScoped<IInterceptor, PerformanceInterceptor>();
        services.AddScoped<IInterceptor, AuditSaveChangesInterceptor>();

        return services;
    }

    public static LoggerConfiguration WriteToMinio(this LoggerConfiguration loggerConfiguration, MinioOptions options)
        => loggerConfiguration.WriteTo.Sink(new MinioS3Sink(options));
}