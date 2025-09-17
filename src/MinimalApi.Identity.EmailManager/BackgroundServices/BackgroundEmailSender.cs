using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.EmailManager.DependencyInjection;
using MinimalApi.Identity.EmailManager.SenderEmail;
using MinimalApi.Identity.EmailManager.Services;

namespace MinimalApi.Identity.EmailManager.BackgroundServices;

public class BackgroundEmailSender(IServiceScopeFactory serviceScopeFactory, IOptions<AppSettings> hostedOptions,
    IOptions<SmtpOptions> smtpOptions, ILogger<BackgroundEmailSender> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new System.Timers.Timer
        {
            Interval = TimeSpan.FromMinutes(hostedOptions.Value.IntervalEmailSenderMinutes).TotalMilliseconds
        };

        timer.Elapsed += Timer_ElapsedAsync;
        timer.Start();

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async void Timer_ElapsedAsync(object? sender, ElapsedEventArgs e)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var emailService = scope.ServiceProvider.GetRequiredService<IEmailManagerService>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IMailKitEmailSender>();

        var emails = emailService.GetAllEmailsAsync();
        var emailsToSend = emails.GetEmailByStatus(EmailStatusType.Pending);

        if (emailsToSend.Any())
        {
            foreach (var email in emailsToSend)
            {
                if (await CheckIfShouldRetryAsync(email, smtpOptions.Value.MaxRetryAttempts, emailService, logger))
                {
                    try
                    {
                        var result = await emailSender.SendEmailAsync(email.EmailTo, email.Subject, email.Body);

                        if (result)
                        {
                            await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Sent, DateTime.UtcNow, email.RetrySender, email.RetrySenderDate);
                        }
                        else
                        {
                            await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Failed, email.DateSent, email.RetrySender + 1, DateTime.UtcNow);
                        }
                    }
                    catch (Exception ex)
                    {
                        email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                        email.RetrySender = email.RetrySender + 1;
                        email.RetrySenderDate = DateTime.UtcNow;
                        email.RetrySenderErrorMessage = "Exception while sending email failed";
                        email.RetrySenderErrorDetails = ex.ToString();

                        await emailService.UpdateEmailAsync(email, CancellationToken.None);
                    }
                }
            }
        }

        await Task.Yield();
    }

    private static async Task<bool> CheckIfShouldRetryAsync(EmailSending email, int maxRetryAttempts, IEmailManagerService emailManagerService,
        ILogger<BackgroundEmailSender> logger)
    {
        var result = false;

        if (email.RetrySender > maxRetryAttempts)
        {
            await emailManagerService.UpdateEmailStatusAsync(email.Id, (int)EmailStatusType.Cancelled, CancellationToken.None);
            logger.LogWarning("Email with Id {EmailId} has exceeded the maximum retry attempts and has been marked as Cancelled.", email.Id);
            result = false;
        }
        else
        {
            logger.LogInformation("Email with Id {EmailId} will be retried. Current attempt: {RetryAttempt}.", email.Id, email.RetrySender + 1);
            result = true;
        }

        return result;
    }

    private static async Task UpdateStatusEmailAsync(IEmailManagerService emailService, EmailSending email, int typeEmailStatusId, DateTime dateSent,
        int retrySender, DateTime? retrySenderDate)
    {
        email.TypeEmailStatusId = typeEmailStatusId;
        email.DateSent = dateSent;
        email.RetrySender = retrySender;
        email.RetrySenderDate = retrySenderDate;

        await emailService.UpdateEmailAsync(email, CancellationToken.None);
    }
}