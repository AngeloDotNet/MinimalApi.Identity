using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi.Identity.API.Configurations;

public class DatabaseServiceConfiguration(IServiceCollection services)
{
    private readonly IServiceCollection services = services;

    public IConfiguration Configure { get; set; } = null!; // Must be set before using
    public string DatabaseType { get; set; } = string.Empty; // Must be set before using
    public string MigrationsAssembly { get; set; } = string.Empty; // Must be set before using
}