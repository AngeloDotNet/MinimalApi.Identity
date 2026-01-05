using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.EmailManager.SenderEmail;

public class MailKitEmailSender(IOptions<SmtpOptions> smtpOptions) : IMailKitEmailSender
{
    public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlBody)
    {
        var options = smtpOptions.Value;

        using SmtpClient client = new();
        MimeMessage message = new();

        try
        {
            await client.ConnectAsync(options.Host, options.Port, options.Security);

            if (!string.IsNullOrEmpty(options.Username))
            {
                await client.AuthenticateAsync(options.Username, options.Password);
            }

            message.From.Add(MailboxAddress.Parse(options.Sender));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            await Task.Delay(1000);
            return await Task.FromResult(true);
        }
        catch
        {
            await Task.Delay(1000);
            return await Task.FromResult(false);
        }
    }
}