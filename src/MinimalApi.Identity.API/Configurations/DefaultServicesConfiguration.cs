using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.API.Configurations;

public class DefaultServicesConfiguration(IServiceCollection services)
{
    private readonly IServiceCollection services = services;

    public string DatabaseType { get; set; } = string.Empty;
    public string MigrationsAssembly { get; set; } = string.Empty;
    public JwtOptions JwtOptions { get; set; } = new JwtOptions();
    public FeatureFlagsOptions FeatureFlags { get; set; } = new FeatureFlagsOptions();
    public ErrorResponseFormat FormatErrorResponse { get; set; } = ErrorResponseFormat.Default;
}