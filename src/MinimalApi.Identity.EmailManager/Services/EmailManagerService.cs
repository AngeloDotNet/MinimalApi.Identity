using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.EmailManager.DependencyInjection;

namespace MinimalApi.Identity.EmailManager.Services;

public class EmailManagerService(MinimalApiAuthDbContext dbContext) : IEmailManagerService
{
    public IQueryable<EmailSending> GetAllEmailsAsync() => EmailQuery.GetAllEmailsAsync(dbContext);

    public async Task<string> GenerateAutomaticEmailAsync(EmailSending model, CancellationToken cancellationToken)
        => await EmailQuery.CreateEmailAsync(model, dbContext, cancellationToken);

    public async Task<string> CreateEmailAsync(EmailSending model, CancellationToken cancellationToken)
        => await EmailQuery.CreateEmailAsync(model, dbContext, cancellationToken);

    public async Task<string> UpdateEmailAsync(EmailSending model, CancellationToken cancellationToken)
        => await EmailQuery.UpdateEmailAsync(model, dbContext, cancellationToken);

    public async Task<string> UpdateEmailStatusAsync(int id, int typeEmailStatusId, CancellationToken cancellationToken)
        => await EmailQuery.UpdateEmailStatusAsync(id, typeEmailStatusId, dbContext, cancellationToken);

    public async Task<string> DeleteEmailAsync(int id, CancellationToken cancellationToken)
        => await EmailQuery.DeleteEmailAsync(id, dbContext, cancellationToken);

    public Task<IQueryable<EmailSending>> GetEmailByStatus(IQueryable<EmailSending> source, EmailStatusType status)
        => Task.FromResult(EmailQuery.GetEmailByStatus(source, status));

    public Task<IQueryable<EmailSending>> GetEmailInDateSentRange(IQueryable<EmailSending> source, DateTime startDate, DateTime endDate)
        => Task.FromResult(EmailQuery.GetEmailInDateSentRange(source, startDate, endDate));

    public Task<IQueryable<EmailSending>> GetEmailToUser(IQueryable<EmailSending> source, string emailTo)
        => Task.FromResult(EmailQuery.GetEmailToUser(source, emailTo));
}