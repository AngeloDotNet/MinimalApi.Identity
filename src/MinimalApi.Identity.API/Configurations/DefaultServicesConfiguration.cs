using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.API.Configurations;

public class DefaultServicesConfiguration(IServiceCollection services)
{
    private readonly IServiceCollection services = services;

    public IConfiguration Configure { get; set; } = null!; // Must be set before using
    public string DatabaseConnectionString { get; set; } = null!; // Must be set before using
    public ErrorResponseFormat FormatErrorResponse { get; set; } = ErrorResponseFormat.Default; // Default format for error responses
}
