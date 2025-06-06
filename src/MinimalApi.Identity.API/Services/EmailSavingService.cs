using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class EmailSavingService(MinimalApiAuthDbContext dbContext) : IEmailSavingService
{
    public async Task SaveEmailAsync(EmailSending email)
    {
        dbContext.Set<EmailSending>().Add(email);
        await dbContext.SaveChangesAsync();
    }
}