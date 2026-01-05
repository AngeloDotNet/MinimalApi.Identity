using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.EmailManager.BackgroundServices;
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
            .AddHostedService<BackgroundEmailSender>();

        return services;
    }
}
