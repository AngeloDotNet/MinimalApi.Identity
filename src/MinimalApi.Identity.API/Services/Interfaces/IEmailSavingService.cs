using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IEmailSavingService
{
    Task SaveEmailAsync(EmailSending email);
}