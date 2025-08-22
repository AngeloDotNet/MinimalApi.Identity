using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.EmailManager.DependencyInjection;
using MinimalApi.Identity.EmailManager.SenderEmail;
using MinimalApi.Identity.EmailManager.Services;

namespace MinimalApi.Identity.EmailManager.HostedServices;

//public class BackgroundEmailSender(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<SmtpOptions> smtpOptions) : IHostedService, IDisposable
public class BackgroundEmailSender(IServiceProvider serviceProvider, IOptions<HostedServiceOptions> hostedOptions, IOptions<SmtpOptions> smtpOptions) : IHostedService, IDisposable
{
    private Timer? timer;
    private readonly SmtpOptions options = smtpOptions.Value;
    private readonly HostedServiceOptions hostedOptions = hostedOptions.Value;
    private CancellationTokenSource cancellationTokenSource = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        //cancellationTokenSource = new CancellationTokenSource();

        //double timerElapsed = configuration.GetValue<int>("HostedServiceOptions:IntervalEmailSenderMinutes");
        double timerElapsed = hostedOptions.IntervalEmailSenderMinutes;
        timer = new Timer(AutomaticEmailSenderAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(timerElapsed));

        return Task.CompletedTask;
    }

    private async void AutomaticEmailSenderAsync(object? state)
    {
        using var scope = serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailManagerService>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IMailKitEmailSender>();

        var emails = await emailService.GetAllEmailsAsync(CancellationToken.None);
        var emailsToSend = EmailQuery.GetEmailByStatus(emails, EmailStatusType.Pending);

        if (emailsToSend.Any())
        {
            foreach (var email in emailsToSend)
            {
                if (email.RetrySender == options.MaxRetryAttempts)
                {
                    await emailService.UpdateEmailStatusAsync(email.Id, (int)EmailStatusType.Cancelled, CancellationToken.None);
                }

                try
                {
                    var result = await emailSender.SendEmailAsync(email.EmailTo, email.Subject, email.Body);

                    if (result)
                    {
                        email.TypeEmailStatusId = (int)EmailStatusType.Sent;
                        email.DateSent = DateTime.UtcNow;

                        await emailService.UpdateEmailAsync(email, CancellationToken.None);
                    }
                    else
                    {
                        email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                        email.RetrySender = email.RetrySender + 1;
                        email.RetrySenderDate = DateTime.UtcNow;

                        await emailService.UpdateEmailAsync(email, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                    email.RetrySender = email.RetrySender + 1;
                    email.RetrySenderDate = DateTime.UtcNow;

                    //email.RetrySenderErrorMessage = ex.Message;
                    email.RetrySenderErrorMessage = "Exception while sending email failed";
                    email.RetrySenderErrorDetails = ex.ToString();

                    await emailService.UpdateEmailAsync(email, CancellationToken.None);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => CancelSendTask();

    private void CancelSendTask()
    {
        try
        {
            if (cancellationTokenSource != null)
            {
                //logger.LogInformation("Stopping e-mail background delivery");
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null!;
            }
        }
        catch
        { }
    }
}