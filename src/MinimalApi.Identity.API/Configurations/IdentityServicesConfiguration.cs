using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Configurations;

public class IdentityServicesConfiguration(IServiceCollection services)
{
    private readonly IServiceCollection services = services;

    public JwtOptions JWTOptions { get; set; } = null!; // Must be set before using
    public NetIdentityOptions IdentityOptions { get; set; } = null!; // Must be set before using
}
