using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.API.Configurations;

public class DefaultServicesConfiguration(IServiceCollection services)
{
    private readonly IServiceCollection services = services;

    public string DatabaseType { get; set; } = string.Empty; // Must be set before using
    public string MigrationsAssembly { get; set; } = string.Empty; // Must be set before using
    public JwtOptions JwtOptions { get; set; } = new JwtOptions(); // JWT options configuration, if needed
    public FeatureFlagsOptions FeatureFlags { get; set; } = new FeatureFlagsOptions(); // Feature flags configuration
    public ErrorResponseFormat FormatErrorResponse { get; set; } = ErrorResponseFormat.Default; // Default format for error responses
}
