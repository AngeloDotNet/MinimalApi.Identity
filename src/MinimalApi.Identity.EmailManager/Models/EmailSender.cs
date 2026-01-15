using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.EmailManager.Models;

public class EmailSender
{
    public EmailSending Email { get; set; } = null!;
    public int MaxRetryAttempts { get; set; } = 0;
}