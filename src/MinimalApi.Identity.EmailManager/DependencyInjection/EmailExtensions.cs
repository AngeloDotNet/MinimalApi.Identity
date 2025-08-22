using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.EmailManager.HostedServices;
using MinimalApi.Identity.EmailManager.SenderEmail;
using MinimalApi.Identity.EmailManager.Services;

namespace MinimalApi.Identity.EmailManager.DependencyInjection;

public static class EmailExtensions
{
    public static IServiceCollection EmailManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IEmailManagerService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .AddSingleton<IMailKitEmailSender, MailKitEmailSender>()
            .AddSingleton<IHostedService, BackgroundEmailSender>();

        return services;
    }
}
