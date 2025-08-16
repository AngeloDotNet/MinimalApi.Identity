using MailKit.Security;

namespace MinimalApi.Identity.Core.Options;

public class SmtpOptions
{
    //[Required]
    public string Host { get; init; } = null!;

    //[IntegerInList(25, 587, 465, ErrorMessage = "Port must be one of the following: 25, 587, 465.")]
    public int Port { get; init; }

    //[Required]
    public SecureSocketOptions Security { get; init; }

    //[Required]
    public string Username { get; init; } = null!;

    //[Required]
    public string Password { get; init; } = null!;

    //[Required]
    //[EmailAddress(ErrorMessage = "Sender must be a valid email address.")]
    public string Sender { get; init; } = null!;

    //public bool SaveEmailSent { get; init; }
    public int MaxRetryAttempts { get; init; } = 10; // Default: 10 attempts
    public TimeSpan RetryDelay { get; init; } = TimeSpan.FromSeconds(30); // Default: 30 seconds delay between retries
}