using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.AuthManager.HostedServices;

namespace MinimalApi.Identity.AuthManager.DependencyInjection;

public static class AuthExtensions
{
    public static IServiceCollection AuthManagerRegistrationService(this IServiceCollection services)
    {
        services
            //.AddRegisterServices(options =>
            //{
            //    options.Interfaces = [typeof(IEmailManagerService)];
            //    options.StringEndsWith = "Service";
            //    options.Lifetime = ServiceLifetime.Transient;
            //})
            //.AddSingleton<IMailKitEmailSender, MailKitEmailSender>()
            .AddHostedService<AuthenticationStartupTask>();

        return services;
    }
}
