using MailKit.Security;

namespace MinimalApi.Identity.Core.Options;

public class SmtpOptions
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public SecureSocketOptions Security { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Sender { get; init; } = null!;
    public int MaxRetryAttempts { get; init; } = 10; // Default: 10 attempts
}