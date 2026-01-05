using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi.Identity.Core.Configurations;

public class ServiceRegistrationConfiguration(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
    public List<Type> Interfaces { get; set; } = [];
    public string StringEndsWith { get; set; } = "Service";
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
}