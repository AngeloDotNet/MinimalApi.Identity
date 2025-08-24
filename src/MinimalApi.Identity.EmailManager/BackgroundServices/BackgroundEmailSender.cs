using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.EmailManager.DependencyInjection;
using MinimalApi.Identity.EmailManager.SenderEmail;
using MinimalApi.Identity.EmailManager.Services;

namespace MinimalApi.Identity.EmailManager.BackgroundServices;

public class BackgroundEmailSender(IServiceScopeFactory serviceScopeFactory, IOptions<HostedServiceOptions> hostedOptions,
    IOptions<SmtpOptions> smtpOptions, ILogger<BackgroundEmailSender> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var timer = new System.Timers.Timer
        {
            Interval = TimeSpan.FromMinutes(hostedOptions.Value.IntervalEmailSenderMinutes).TotalMilliseconds
        };

        timer.Elapsed += Timer_ElapsedAsync;
        timer.Start();

        await Task.Delay(Timeout.Infinite, cancellationToken);
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
                if (email.RetrySender > smtpOptions.Value.MaxRetryAttempts)
                {
                    await emailService.UpdateEmailStatusAsync(email.Id, (int)EmailStatusType.Cancelled, CancellationToken.None);
                    logger.LogWarning("Email with Id {EmailId} has been cancelled after reaching max retry attempts.", email.Id);

                    continue;
                }

                if (email.RetrySender <= smtpOptions.Value.MaxRetryAttempts)
                {
                    try
                    {
                        var result = await emailSender.SendEmailAsync(email.EmailTo, email.Subject, email.Body);

                        if (result)
                        {
                            //email.TypeEmailStatusId = (int)EmailStatusType.Sent;
                            //email.DateSent = DateTime.UtcNow;
                            //email.RetrySender = email.RetrySender;
                            //email.RetrySenderDate = email.RetrySenderDate;

                            //await emailService.UpdateEmailAsync(email, CancellationToken.None);
                            await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Sent, DateTime.UtcNow, email.RetrySender, email.RetrySenderDate);
                        }
                        else
                        {
                            //email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                            //email.DateSent = email.DateSent;
                            //email.RetrySender = email.RetrySender + 1;
                            //email.RetrySenderDate = DateTime.UtcNow;

                            //await emailService.UpdateEmailAsync(email, CancellationToken.None);
                            await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Failed, email.DateSent, email.RetrySender + 1, DateTime.UtcNow);
                        }
                    }
                    catch (Exception ex)
                    {
                        email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                        //email.RetrySender++;
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

    //private Timer? timer;
    //private readonly SmtpOptions options = smtpOptions.Value;
    //private readonly HostedServiceOptions hostedOptions = hostedOptions.Value;

    //public Task StartAsync(CancellationToken cancellationToken)
    //{
    //    timer = new Timer(AutomaticEmailSenderAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(hostedOptions.IntervalEmailSenderMinutes));
    //    return Task.CompletedTask;
    //}

    //private async void AutomaticEmailSenderAsync(object? state)
    //{
    //    using var scope = serviceProvider.CreateScope();

    //    var emailService = scope.ServiceProvider.GetRequiredService<IEmailManagerService>();
    //    var emailSender = scope.ServiceProvider.GetRequiredService<IMailKitEmailSender>();

    //    var emails = await emailService.GetAllEmailsAsync(CancellationToken.None);
    //    var emailsToSend = emails.GetEmailByStatus(EmailStatusType.Pending);

    //    if (emailsToSend.Any())
    //    {
    //        foreach (var email in emailsToSend)
    //        {
    //            if (email.RetrySender == options.MaxRetryAttempts)
    //            {
    //                await emailService.UpdateEmailStatusAsync(email.Id, (int)EmailStatusType.Cancelled, CancellationToken.None);
    //                continue;
    //            }

    //            try
    //            {
    //                var result = await emailSender.SendEmailAsync(email.EmailTo, email.Subject, email.Body);

    //                if (result)
    //                {
    //                    //email.TypeEmailStatusId = (int)EmailStatusType.Sent;
    //                    //email.DateSent = DateTime.UtcNow;
    //                    //email.RetrySender = email.RetrySender;
    //                    //email.RetrySenderDate = email.RetrySenderDate;

    //                    //await emailService.UpdateEmailAsync(email, CancellationToken.None);
    //                    await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Sent, DateTime.UtcNow, email.RetrySender, email.RetrySenderDate);
    //                }
    //                else
    //                {
    //                    //email.TypeEmailStatusId = (int)EmailStatusType.Failed;
    //                    //email.DateSent = email.DateSent;
    //                    //email.RetrySender = email.RetrySender + 1;
    //                    //email.RetrySenderDate = DateTime.UtcNow;

    //                    //await emailService.UpdateEmailAsync(email, CancellationToken.None);
    //                    await UpdateStatusEmailAsync(emailService, email, (int)EmailStatusType.Failed, email.DateSent, email.RetrySender + 1, DateTime.UtcNow);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                email.TypeEmailStatusId = (int)EmailStatusType.Failed;
    //                //email.RetrySender++;
    //                email.RetrySender = email.RetrySender + 1;
    //                email.RetrySenderDate = DateTime.UtcNow;
    //                email.RetrySenderErrorMessage = "Exception while sending email failed";
    //                email.RetrySenderErrorDetails = ex.ToString();

    //                await emailService.UpdateEmailAsync(email, CancellationToken.None);
    //            }
    //        }
    //    }
    //}

    private static async Task UpdateStatusEmailAsync(IEmailManagerService emailService, EmailSending email, int typeEmailStatusId, DateTime dateSent,
        int retrySender, DateTime? retrySenderDate)
    {
        email.TypeEmailStatusId = typeEmailStatusId;
        email.DateSent = dateSent;
        email.RetrySender = retrySender;
        email.RetrySenderDate = retrySenderDate;

        await emailService.UpdateEmailAsync(email, CancellationToken.None);
    }

    //public Task StopAsync(CancellationToken cancellationToken)
    //{
    //    timer?.Change(Timeout.Infinite, 0);
    //    return Task.CompletedTask;
    //}
}