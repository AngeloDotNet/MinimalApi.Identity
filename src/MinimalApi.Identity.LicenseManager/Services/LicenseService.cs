using System.Security.Claims;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.LicenseManager.DependencyInjection;
using MinimalApi.Identity.LicenseManager.Models;

namespace MinimalApi.Identity.LicenseManager.Services;

public class LicenseService(MinimalApiAuthDbContext dbContext) : ILicenseService
{
    public async Task<List<LicenseResponseModel>> GetAllLicensesAsync(CancellationToken cancellationToken)
        => await LicenseQuery.GetAllLicensesAsync(dbContext, cancellationToken);
    //{
    //    var licenses = await dbContext.Set<License>()
    //        .AsNoTracking()
    //        .ToLicenseResponseModel()
    //        .ToListAsync(cancellationToken);

    //    return licenses.Count == 0 ? [] : licenses;
    //}

    public async Task<string> CreateLicenseAsync(CreateLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.CreateLicenseAsync(model, dbContext, cancellationToken);
    //{
    //    if (await CheckLicenseExistAsync(model, cancellationToken))
    //    {
    //        throw new ConflictException(MessagesAPI.LicenseAlreadyExist);
    //    }

    //    var license = new License
    //    {
    //        Name = model.Name,
    //        ExpirationDate = model.ExpirationDate
    //    };

    //    dbContext.Set<License>().Add(license);
    //    await dbContext.SaveChangesAsync(cancellationToken);

    //    return MessagesAPI.LicenseCreated;
    //}

    public async Task<string> AssignLicenseAsync(AssignLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.AssignLicenseAsync(model, dbContext, cancellationToken);
    //{
    //    var userHasLicense = await dbContext.Set<UserLicense>()
    //        .AsNoTracking()
    //        .AnyAsync(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId, cancellationToken);

    //    if (userHasLicense)
    //    {
    //        throw new BadRequestException(MessagesAPI.LicenseNotAssignable);
    //    }

    //    var userLicense = new UserLicense
    //    {
    //        UserId = model.UserId,
    //        LicenseId = model.LicenseId
    //    };

    //    dbContext.Set<UserLicense>().Add(userLicense);
    //    await dbContext.SaveChangesAsync(cancellationToken);

    //    return MessagesAPI.LicenseAssigned;
    //}

    public async Task<string> RevokeLicenseAsync(RevokeLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.RevokeLicenseAsync(model, dbContext, cancellationToken);
    //{
    //    var userLicense = await dbContext.Set<UserLicense>()
    //        .AsNoTracking()
    //        .Where(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId)
    //        .ToUserLicense()
    //        .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesAPI.LicenseNotFound);

    //    dbContext.Set<UserLicense>().Remove(userLicense);
    //    await dbContext.SaveChangesAsync(cancellationToken);

    //    return MessagesAPI.LicenseCanceled;
    //}

    public async Task<string> DeleteLicenseAsync(DeleteLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.DeleteLicenseAsync(model, dbContext, cancellationToken);
    //{
    //    var license = await dbContext.Set<License>()
    //        .AsNoTracking()
    //        .Where(x => x.Id == model.LicenseId)
    //        .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesAPI.LicenseNotFound);

    //    if (await dbContext.Set<UserLicense>().AsNoTracking().AnyAsync(ul => ul.LicenseId == model.LicenseId, cancellationToken))
    //    {
    //        throw new BadRequestException(MessagesAPI.LicenseNotDeleted);
    //    }

    //    dbContext.Set<License>().Remove(license);
    //    await dbContext.SaveChangesAsync(cancellationToken);

    //    return MessagesAPI.LicenseDeleted;
    //}

    public async Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user, CancellationToken cancellationToken)
        => await LicenseQuery.GetClaimLicenseUserAsync(user, dbContext, cancellationToken);
    //{
    //    var result = await dbContext.Set<UserLicense>()
    //        .AsNoTracking()
    //        .Where(ul => ul.UserId == user.Id)
    //        .ToUserLicense()
    //        .FirstOrDefaultAsync(cancellationToken);

    //    return result != null ? new Claim(LicenseExtensions.License, result.License.Name) : null!;
    //}

    public async Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user, CancellationToken cancellationToken)
        => await LicenseQuery.CheckUserLicenseExpiredAsync(user, dbContext, cancellationToken);
    //{
    //    return await dbContext.Set<UserLicense>()
    //        .AsNoTracking()
    //        .Include(ul => ul.License)
    //        .AnyAsync(ul => ul.UserId == user.Id && ul.License.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
    //}
}