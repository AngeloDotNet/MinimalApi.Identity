using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.AuthManager.HostedServices;

namespace MinimalApi.Identity.AuthManager.DependencyInjection;

public static class AuthExtensions
{
    public static IServiceCollection AuthManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddHostedService<AuthenticationStartupTask>();

        return services;
    }
}
