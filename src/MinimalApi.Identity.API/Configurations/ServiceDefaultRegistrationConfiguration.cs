using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;

namespace MinimalApi.Identity.API.Configurations;

public class ServiceDefaultRegistrationConfiguration(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
    public IConfiguration Configuration { get; set; }
    public DatabaseType TypeDatabase { get; set; }
    public ErrorResponseFormat ErrorResponseFormat { get; set; } = ErrorResponseFormat.Default;
    public AppSettings ApplicationSettings { get; set; }
    public JwtOptions JwtSettings { get; set; }
    public CorsOptions CorsSettings { get; set; }
    public FeatureFlagsOptions ActiveModules { get; set; }
    public SmtpOptions SmtpSettings { get; set; }
    public string MigrationsAssembly { get; set; } = string.Empty;
}