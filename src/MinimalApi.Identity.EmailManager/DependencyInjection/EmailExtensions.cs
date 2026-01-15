using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.EmailManager.BackgroundServices;
using MinimalApi.Identity.EmailManager.Models;
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

    public static async Task<bool> CheckIfShouldRetryAsync(EmailSender email, IEmailManagerService emailManagerService, ILogger<BackgroundEmailSender> logger)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(emailManagerService);
        ArgumentNullException.ThrowIfNull(logger);

        if (email.Email.RetrySender > email.MaxRetryAttempts)
        {
            await emailManagerService.UpdateEmailStatusAsync(email.Email.Id, (int)EmailStatusType.Cancelled, CancellationToken.None)
                .ConfigureAwait(false);

            logger.LogWarning(
                "Email with Id {EmailId} has exceeded the maximum retry attempts ({MaxRetries}) and has been marked as Cancelled.",
                email.Email.Id, email.MaxRetryAttempts);

            return false;
        }

        logger.LogInformation("Email with Id {EmailId} will be retried. Current attempt: {RetryAttempt}.",
            email.Email.Id, email.Email.RetrySender + 1);

        return true;
    }
}
