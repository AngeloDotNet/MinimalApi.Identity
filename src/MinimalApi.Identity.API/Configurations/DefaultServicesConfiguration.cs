using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.API.Configurations;

public class DefaultServicesConfiguration(IServiceCollection services)
{
    private readonly IServiceCollection services = services;

    public string MigrationsAssembly { get; set; } = string.Empty; // Must be set before using
    public ErrorResponseFormat FormatErrorResponse { get; set; } = ErrorResponseFormat.Default; // Default format for error responses
}
