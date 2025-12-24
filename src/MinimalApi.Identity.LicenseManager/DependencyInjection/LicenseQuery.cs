using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.LicenseManager.Extensions;
using MinimalApi.Identity.LicenseManager.Models;

namespace MinimalApi.Identity.LicenseManager.DependencyInjection;

public static class LicenseQuery
{
    public static async Task<List<LicenseResponseModel>> GetAllLicensesAsync(MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var licenses = await dbContext.Set<License>()
            .ToLicenseResponseModel()
            .ToListAsync(cancellationToken);

        return licenses.Count == 0 ? [] : licenses;
    }

    public static async Task<string> CreateLicenseAsync(CreateLicenseModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await CheckLicenseExistAsync(model, dbContext, cancellationToken))
        {
            throw new ConflictException(MessagesApi.LicenseAlreadyExist);
        }

        var license = new License
        {
            Name = model.Name,
            ExpirationDate = model.ExpirationDate
        };

        dbContext.Set<License>().Add(license);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.LicenseCreated;
    }

    public static async Task<string> AssignLicenseAsync(AssignLicenseModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var userHasLicense = await dbContext.Set<UserLicense>()
            .AnyAsync(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId, cancellationToken);

        if (userHasLicense)
        {
            throw new BadRequestException(MessagesApi.LicenseNotAssignable);
        }

        var userLicense = new UserLicense
        {
            UserId = model.UserId,
            LicenseId = model.LicenseId
        };

        dbContext.Set<UserLicense>().Add(userLicense);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.LicenseAssigned;
    }

    public static async Task<string> RevokeLicenseAsync(RevokeLicenseModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var userLicense = await dbContext.Set<UserLicense>()
            .Where(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId)
            .ToUserLicense()
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.LicenseNotFound);

        dbContext.Set<UserLicense>().Remove(userLicense);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.LicenseCanceled;
    }

    public static async Task<string> DeleteLicenseAsync(DeleteLicenseModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var license = await dbContext.Set<License>()
            .Where(x => x.Id == model.LicenseId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.LicenseNotFound);

        dbContext.Set<License>().Remove(license);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.LicenseDeleted;
    }

    public static async Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var result = await dbContext.Set<UserLicense>()
            .Where(ul => ul.UserId == user.Id)
            .ToUserLicense()
            .FirstOrDefaultAsync(cancellationToken);

        return result != null ? new Claim(LicenseExtensions.License, result.License.Name) : null!;
    }

    public static async Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Set<UserLicense>()
            .Include(ul => ul.License)
            .AnyAsync(ul => ul.UserId == user.Id && ul.License.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
    }

    private static async Task<bool> CheckLicenseExistAsync(CreateLicenseModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
        => await dbContext.Set<License>()
        .AnyAsync(l => l.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
}