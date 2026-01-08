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
    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    var timer = new System.Timers.Timer
    //    {
    //        Interval = TimeSpan.FromMinutes(hostedOptions.Value.IntervalEmailSenderMinutes).TotalMilliseconds
    //    };

    //    timer.Elapsed += Timer_ElapsedAsync;
    //    timer.Start();

    //    await Task.Delay(Timeout.Infinite, stoppingToken);
    //}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = hostedOptions.Value.IntervalEmailSenderMinutes;
        var interval = TimeSpan.FromMinutes(Math.Max(0.1, intervalMinutes));

        await Task.Yield();
        using var periodicTimer = new PeriodicTimer(interval);

        try
        {
            while (await periodicTimer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
            {
                _ = Task.Run(() => Timer_ElapsedAsync(null, null), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when stoppingToken is cancelled — swallow to allow graceful shutdown.
        }
    }

    private async void Timer_ElapsedAsync(object? sender, ElapsedEventArgs? e)
    {
        //using var scope = serviceScopeFactory.CreateScope();

        //var emailService = scope.ServiceProvider.GetRequiredService<IEmailManagerService>();
        //var emailSender = scope.ServiceProvider.GetRequiredService<IMailKitEmailSender>();

        //var emails = emailService.GetAllEmailsAsync();
        //var emailsToSend = emails.GetEmailByStatus(EmailStatusType.Pending);

        using var scope = serviceScopeFactory.CreateScope();

        var provider = scope.ServiceProvider;
        var emailService = provider.GetRequiredService<IEmailManagerService>();
        var emailSender = provider.GetRequiredService<IMailKitEmailSender>();

        var emails = emailService.GetAllEmailsAsync();
        var emailsToSend = emails.GetEmailByStatus(EmailStatusType.Pending);

        if (emailsToSend.Any())
        {
            //foreach (var email in emailsToSend)
            //{
            //    if (await CheckIfShouldRetryAsync(email, smtpOptions.Value.MaxRetryAttempts, emailService, logger))
            //    {
            //        try
            //        {
            //            var result = await emailSender.SendEmailAsync(email.EmailTo, email.Subject, email.Body);

            //            if (result)
            //            {
            //                await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Sent, DateTime.UtcNow, email.RetrySender, email.RetrySenderDate);
            //            }
            //            else
            //            {
            //                await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Failed, email.DateSent, email.RetrySender + 1, DateTime.UtcNow);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            email.TypeEmailStatusId = (int)EmailStatusType.Failed;
            //            email.RetrySender = email.RetrySender + 1;
            //            email.RetrySenderDate = DateTime.UtcNow;
            //            email.RetrySenderErrorMessage = "Exception while sending email failed";
            //            email.RetrySenderErrorDetails = ex.ToString();

            //            await emailService.UpdateEmailAsync(email, CancellationToken.None);
            //        }
            //    }
            //}

            var maxRetryAttempts = smtpOptions.Value.MaxRetryAttempts;

            foreach (var email in emailsToSend)
            {
                var emailSenderOptions = new EmailSender()
                {
                    Email = email,
                    MaxRetryAttempts = maxRetryAttempts
                };

                //if (!await CheckIfShouldRetryAsync(email, maxRetryAttempts, emailService, logger).ConfigureAwait(false))
                if (!await CheckIfShouldRetryAsync(emailSenderOptions, emailService, logger).ConfigureAwait(false))
                {
                    continue;
                }

                try
                {
                    var sent = await emailSender.SendEmailAsync(email.EmailTo, email.Subject, email.Body).ConfigureAwait(false);

                    if (sent)
                    {
                        email.TypeEmailStatusId = (int)EmailStatusType.Sent;
                        email.DateSent = DateTime.UtcNow;
                        // keep retry values as they are on success
                    }
                    else
                    {
                        email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                        // preserve original DateSent; increment retry metadata
                        email.RetrySender = email.RetrySender + 1;
                        email.RetrySenderDate = DateTime.UtcNow;
                    }

                    await emailService.UpdateEmailAsync(email, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                    email.RetrySender = email.RetrySender + 1;
                    email.RetrySenderDate = DateTime.UtcNow;
                    email.RetrySenderErrorMessage = "Exception while sending email failed";
                    email.RetrySenderErrorDetails = ex.ToString();

                    await emailService.UpdateEmailAsync(email, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        await Task.Yield();
    }

    //private static async Task<bool> CheckIfShouldRetryAsync(EmailSending email, int maxRetryAttempts, IEmailManagerService emailManagerService,
    private static async Task<bool> CheckIfShouldRetryAsync(EmailSender email, IEmailManagerService emailManagerService, ILogger<BackgroundEmailSender> logger)
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

    internal class EmailSender
    {
        public EmailSending Email { get; set; } = null!;
        public int MaxRetryAttempts { get; set; } = 0;
    }

    //private static async Task UpdateStatusEmailAsync(IEmailManagerService emailService, EmailSending email, int typeEmailStatusId, DateTime dateSent,
    //    int retrySender, DateTime? retrySenderDate)
    //{
    //    email.TypeEmailStatusId = typeEmailStatusId;
    //    email.DateSent = dateSent;
    //    email.RetrySender = retrySender;
    //    email.RetrySenderDate = retrySenderDate;

    //    await emailService.UpdateEmailAsync(email, CancellationToken.None);
    //}
}