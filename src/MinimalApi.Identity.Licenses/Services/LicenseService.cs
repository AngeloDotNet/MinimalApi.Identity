using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Licenses.DependencyInjection;
using MinimalApi.Identity.Licenses.Extensions;
using MinimalApi.Identity.Licenses.Models;
using MinimalApi.Identity.Licenses.Services.Interfaces;

namespace MinimalApi.Identity.Licenses.Services;

public class LicenseService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : ILicenseService
{
    public async Task<List<LicenseResponseModel>> GetAllLicensesAsync()
    {
        //var licenses = await dbContext.Set<License>()
        //    .Select(l => new LicenseResponseModel(l.Id, l.Name, l.ExpirationDate))
        //    .ToListAsync();

        var licenses = await dbContext.Set<License>()
            .AsNoTracking()
            .ToLicenseResponseModel()
            .ToListAsync();

        return licenses.Count == 0 ? [] : licenses;
    }

    public async Task<string> CreateLicenseAsync(CreateLicenseModel model)
    {
        if (await CheckLicenseExistAsync(model))
        {
            throw new ConflictException(LicenseExtensions.LicenseAlreadyExist);
        }

        var license = new License
        {
            Name = model.Name,
            ExpirationDate = model.ExpirationDate
        };

        dbContext.Set<License>().Add(license);
        await dbContext.SaveChangesAsync();

        return LicenseExtensions.LicenseCreated;
    }

    public async Task<string> AssignLicenseAsync(AssignLicenseModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundException(ServiceCoreExtensions.UserNotFound);

        var license = await dbContext.Set<License>().FindAsync(model.LicenseId)
            ?? throw new NotFoundException(LicenseExtensions.LicenseNotFound);

        var userHasLicense = await dbContext.Set<UserLicense>()
            .AnyAsync(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId);

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
        await dbContext.SaveChangesAsync();

        return LicenseExtensions.LicenseAssigned;
    }

    public async Task<string> RevokeLicenseAsync(RevokeLicenseModel model)
    {
        var userLicense = await dbContext.Set<UserLicense>()
            .SingleOrDefaultAsync(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId)
            ?? throw new NotFoundException(LicenseExtensions.LicenseNotFound);

        dbContext.Set<UserLicense>().Remove(userLicense);
        await dbContext.SaveChangesAsync();

        return LicenseExtensions.LicenseCanceled;
    }

    public async Task<string> DeleteLicenseAsync(DeleteLicenseModel model)
    {
        var license = await dbContext.Set<License>().FindAsync(model.LicenseId)
            ?? throw new NotFoundException(LicenseExtensions.LicenseNotFound);

        if (await dbContext.Set<UserLicense>().AnyAsync(ul => ul.LicenseId == model.LicenseId))
        {
            throw new BadRequestException(LicenseExtensions.LicenseNotDeleted);
        }

        dbContext.Set<License>().Remove(license);
        await dbContext.SaveChangesAsync();

        return LicenseExtensions.LicenseDeleted;
    }

    public async Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user)
    {
        //var result = await dbContext.Set<UserLicense>()
        //    .AsNoTracking()
        //    .Include(ul => ul.License)
        //    .FirstOrDefaultAsync(ul => ul.UserId == user.Id);

        var result = await dbContext.Set<UserLicense>()
            .AsNoTracking()
            .Where(ul => ul.UserId == user.Id)
            .ToUserLicense()
            .FirstOrDefaultAsync();

        return result != null ? new Claim(LicenseExtensions.License, result.License.Name) : null!;
    }

    public async Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user)
    {
        return await dbContext.Set<UserLicense>()
            .AsNoTracking()
            .Include(ul => ul.License)
            .AnyAsync(ul => ul.UserId == user.Id && ul.License.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow));
    }

    private async Task<bool> CheckLicenseExistAsync(CreateLicenseModel model)
        => await dbContext.Set<License>().AnyAsync(l => l.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase));
}
