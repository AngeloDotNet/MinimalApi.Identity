using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.API.Services;

public class EmailSavingService(MinimalApiAuthDbContext dbContext) : IEmailSavingService
{
    public async Task SaveEmailAsync(EmailSending email)
    {
        dbContext.Set<EmailSending>().Add(email);
        await dbContext.SaveChangesAsync();
    }
}