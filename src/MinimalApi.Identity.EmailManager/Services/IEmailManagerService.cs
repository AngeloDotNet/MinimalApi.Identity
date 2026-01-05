using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.EmailManager.Services;

public interface IEmailManagerService
{
    IQueryable<EmailSending> GetAllEmailsAsync();
    Task<string> GenerateAutomaticEmailAsync(EmailSending model, CancellationToken cancellationToken);
    Task<string> CreateEmailAsync(EmailSending model, CancellationToken cancellationToken);
    Task<string> UpdateEmailAsync(EmailSending model, CancellationToken cancellationToken);
    Task<string> UpdateEmailStatusAsync(int id, int typeEmailStatusId, CancellationToken cancellationToken);
    Task<string> DeleteEmailAsync(int id, CancellationToken cancellationToken);
    Task<IQueryable<EmailSending>> GetEmailInDateSentRange(IQueryable<EmailSending> source, DateTime startDate, DateTime endDate);
    Task<IQueryable<EmailSending>> GetEmailToUser(IQueryable<EmailSending> source, string emailTo);
    Task<IQueryable<EmailSending>> GetEmailByStatus(IQueryable<EmailSending> source, EmailStatusType status);
}