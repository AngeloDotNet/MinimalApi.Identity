using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.EmailManager.DependencyInjection;

public static class EmailQuery
{
    public static IQueryable<EmailSending> GetAllEmailsAsync(MinimalApiAuthDbContext dbContext)
        => dbContext.Set<EmailSending>();

    public static async Task<string> CreateEmailAsync(EmailSending model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var emailToSend = new EmailSending
        {
            EmailTo = model.EmailTo,
            Subject = model.Subject,
            Body = model.Body,
            TypeEmailSendingId = model.TypeEmailSendingId,
            TypeEmailStatusId = (int)EmailStatusType.Pending,
            DateSent = DateTime.UtcNow,
            RetrySender = 0,
        };

        dbContext.Set<EmailSending>().Add(emailToSend);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.EmailCreated;
    }

    public static async Task<string> UpdateEmailAsync(EmailSending model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var emailToUpdate = await dbContext.Set<EmailSending>().Where(x => x.Id == model.Id)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.EmailNotFound);

        emailToUpdate.EmailTo = model.EmailTo;
        emailToUpdate.Subject = model.Subject;
        emailToUpdate.Body = model.Body;
        emailToUpdate.TypeEmailSendingId = model.TypeEmailSendingId;
        emailToUpdate.TypeEmailStatusId = model.TypeEmailStatusId;
        emailToUpdate.DateSent = model.DateSent;
        emailToUpdate.RetrySender = model.RetrySender;
        emailToUpdate.RetrySenderDate = model.RetrySenderDate;
        emailToUpdate.RetrySenderErrorMessage = model.RetrySenderErrorMessage;
        emailToUpdate.RetrySenderErrorDetails = model.RetrySenderErrorDetails;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.EmailUpdated;
    }

    public static async Task<string> UpdateEmailStatusAsync(int id, int typeEmailStatusId, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var emailToUpdate = await dbContext.Set<EmailSending>().Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.EmailNotFound);

        emailToUpdate.TypeEmailStatusId = typeEmailStatusId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.EmailUpdated;
    }

    public static async Task<string> DeleteEmailAsync(int id, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var emailToDelete = await dbContext.Set<EmailSending>().Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException(MessagesApi.EmailNotFound);

        emailToDelete.TypeEmailStatusId = (int)EmailStatusType.Cancelled;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.EmailDeleted;
    }

    public static IQueryable<EmailSending> GetEmailInDateSentRange(this IQueryable<EmailSending> source, DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be greater than end date.", nameof(startDate));
        }

        return source.Where(c => c.DateSent >= startDate && c.DateSent <= endDate);
    }

    public static IQueryable<EmailSending> GetEmailToUser(this IQueryable<EmailSending> source, string emailTo)
    {
        if (string.IsNullOrWhiteSpace(emailTo))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(emailTo));
        }

        return source.Where(c => c.EmailTo == emailTo);
    }

    public static IQueryable<EmailSending> GetEmailByStatus(this IQueryable<EmailSending> source, EmailStatusType status)
    {
        return status switch
        {
            EmailStatusType.Sent => source.GetEmailSending(),
            EmailStatusType.Pending => source.GetEmailPending(),
            EmailStatusType.Failed => source.GetEmailFailed(),
            EmailStatusType.Cancelled => source.GetEmailCancelled(),
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    internal static IQueryable<EmailSending> GetEmailSending(this IQueryable<EmailSending> source)
        => source.Where(c => c.TypeEmailStatusId == (int)EmailStatusType.Sent);

    internal static IQueryable<EmailSending> GetEmailPending(this IQueryable<EmailSending> source)
        => source.Where(c => c.TypeEmailStatusId == (int)EmailStatusType.Pending);

    internal static IQueryable<EmailSending> GetEmailFailed(this IQueryable<EmailSending> source)
        => source.Where(c => c.TypeEmailStatusId == (int)EmailStatusType.Failed);

    internal static IQueryable<EmailSending> GetEmailCancelled(this IQueryable<EmailSending> source)
        => source.Where(c => c.TypeEmailStatusId == (int)EmailStatusType.Cancelled);
}