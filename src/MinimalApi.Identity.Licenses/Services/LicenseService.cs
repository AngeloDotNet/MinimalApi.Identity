using System.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Licenses.DependencyInjection;
using MinimalApi.Identity.Licenses.Extensions;
using MinimalApi.Identity.Licenses.Models;
using MinimalApi.Identity.Licenses.Services.Interfaces;

namespace MinimalApi.Identity.Licenses.Services;

public class LicenseService(MinimalApiAuthDbContext dbContext) : ILicenseService
{
    public async Task<List<LicenseResponseModel>> GetAllLicensesAsync(CancellationToken cancellationToken)
    {
        var licenses = await dbContext.Set<License>()
            .AsNoTracking()
            .ToLicenseResponseModel()
            .ToListAsync(cancellationToken);

        return licenses.Count == 0 ? [] : licenses;
    }

    public async Task<string> CreateLicenseAsync(CreateLicenseModel model, CancellationToken cancellationToken)
    {
        if (await CheckLicenseExistAsync(model, cancellationToken))
        {
            throw new ConflictException(LicenseExtensions.LicenseAlreadyExist);
        }

        var license = new License
        {
            Name = model.Name,
            ExpirationDate = model.ExpirationDate
        };

        dbContext.Set<License>().Add(license);
        await dbContext.SaveChangesAsync(cancellationToken);

        return LicenseExtensions.LicenseCreated;
    }

    public async Task<string> AssignLicenseAsync(AssignLicenseModel model, CancellationToken cancellationToken)
    {
        var userHasLicense = await dbContext.Set<UserLicense>()
            .AsNoTracking()
            .AnyAsync(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId, cancellationToken);

        if (userHasLicense)
        {
            throw new BadRequestException(LicenseExtensions.LicenseNotAssignable);
        }

        var userLicense = new UserLicense
        {
            UserId = model.UserId,
            LicenseId = model.LicenseId
        };

        dbContext.Set<UserLicense>().Add(userLicense);
        await dbContext.SaveChangesAsync(cancellationToken);

        return LicenseExtensions.LicenseAssigned;
    }

    public async Task<string> RevokeLicenseAsync(RevokeLicenseModel model, CancellationToken cancellationToken)
    {
        var userLicense = await dbContext.Set<UserLicense>()
            .AsNoTracking()
            .Where(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId)
            .ToUserLicense()
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(LicenseExtensions.LicenseNotFound);

        dbContext.Set<UserLicense>().Remove(userLicense);
        await dbContext.SaveChangesAsync(cancellationToken);

        return LicenseExtensions.LicenseCanceled;
    }

    public async Task<string> DeleteLicenseAsync(DeleteLicenseModel model, CancellationToken cancellationToken)
    {
        var license = await dbContext.Set<License>()
            .AsNoTracking()
            .Where(x => x.Id == model.LicenseId)
            .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(LicenseExtensions.LicenseNotFound);

        if (await dbContext.Set<UserLicense>().AsNoTracking().AnyAsync(ul => ul.LicenseId == model.LicenseId, cancellationToken))
        {
            throw new BadRequestException(LicenseExtensions.LicenseNotDeleted);
        }

        dbContext.Set<License>().Remove(license);
        await dbContext.SaveChangesAsync(cancellationToken);

        return LicenseExtensions.LicenseDeleted;
    }

    public async Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var result = await dbContext.Set<UserLicense>()
            .AsNoTracking()
            .Where(ul => ul.UserId == user.Id)
            .ToUserLicense()
            .FirstOrDefaultAsync(cancellationToken);

        return result != null ? new Claim(LicenseExtensions.License, result.License.Name) : null!;
    }

    public async Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return await dbContext.Set<UserLicense>()
            .AsNoTracking()
            .Include(ul => ul.License)
            .AnyAsync(ul => ul.UserId == user.Id && ul.License.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
    }

    private async Task<bool> CheckLicenseExistAsync(CreateLicenseModel model, CancellationToken cancellationToken)
        => await dbContext.Set<License>()
            .AsNoTracking()
            .AnyAsync(l => l.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
}
