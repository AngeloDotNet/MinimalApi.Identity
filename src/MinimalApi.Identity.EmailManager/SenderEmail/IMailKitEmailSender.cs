namespace MinimalApi.Identity.EmailManager.SenderEmail;

public interface IMailKitEmailSender
{
    Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlBody);
}
