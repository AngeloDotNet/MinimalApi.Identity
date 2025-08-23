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

public class BackgroundEmailSender(IServiceProvider serviceProvider, IOptions<HostedServiceOptions> hostedOptions, IOptions<SmtpOptions> smtpOptions) : IHostedService, IDisposable
{
    private Timer? timer;
    private CancellationTokenSource? cancellationTokenSource;

    private readonly SmtpOptions options = smtpOptions.Value;
    private readonly HostedServiceOptions hostedOptions = hostedOptions.Value;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationTokenSource = new CancellationTokenSource();
        timer = new Timer(AutomaticEmailSenderAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(hostedOptions.IntervalEmailSenderMinutes));
        return Task.CompletedTask;
    }

    // Avoid async void, use Task for better error handling and await in Timer callback
    private async Task AutomaticEmailSenderAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailManagerService>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IMailKitEmailSender>();

        var emails = await emailService.GetAllEmailsAsync(CancellationToken.None).ConfigureAwait(false);
        var emailsToSend = EmailQuery.GetEmailByStatus(emails, EmailStatusType.Pending);

        //if (emailsToSend.Count > 0)
        if (emailsToSend.Any())
        {
            foreach (var email in emailsToSend)
            {
                if (email.RetrySender == options.MaxRetryAttempts)
                {
                    await emailService.UpdateEmailStatusAsync(email.Id, (int)EmailStatusType.Cancelled, CancellationToken.None).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    var result = await emailSender.SendEmailAsync(email.EmailTo, email.Subject, email.Body).ConfigureAwait(false);

                    email.TypeEmailStatusId = result ? (int)EmailStatusType.Sent : (int)EmailStatusType.Failed;
                    email.DateSent = result ? DateTime.UtcNow : email.DateSent;
                    email.RetrySender = result ? email.RetrySender : email.RetrySender + 1;
                    email.RetrySenderDate = result ? email.RetrySenderDate : DateTime.UtcNow;

                    await emailService.UpdateEmailAsync(email, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    email.TypeEmailStatusId = (int)EmailStatusType.Failed;
                    email.RetrySender++;
                    email.RetrySenderDate = DateTime.UtcNow;
                    email.RetrySenderErrorMessage = "Exception while sending email failed";
                    email.RetrySenderErrorDetails = ex.ToString();

                    await emailService.UpdateEmailAsync(email, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }
    }

    // Timer callback wrapper to run async code safely
    private void AutomaticEmailSenderAsync(object? state)
    {
        _ = AutomaticEmailSenderAsync();
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
            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;
            }
        }
        catch
        {
            // Swallow exceptions to avoid throwing on dispose
        }
    }
}