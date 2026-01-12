using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.EmailManager.Models;

internal class EmailSender
{
    public EmailSending Email { get; set; } = null!;
    public int MaxRetryAttempts { get; set; } = 0;
}