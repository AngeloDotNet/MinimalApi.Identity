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

    public async Task<string> CreateLicenseAsync(CreateLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.CreateLicenseAsync(model, dbContext, cancellationToken);

    public async Task<string> AssignLicenseAsync(AssignLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.AssignLicenseAsync(model, dbContext, cancellationToken);

    public async Task<string> RevokeLicenseAsync(RevokeLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.RevokeLicenseAsync(model, dbContext, cancellationToken);

    public async Task<string> DeleteLicenseAsync(DeleteLicenseModel model, CancellationToken cancellationToken)
        => await LicenseQuery.DeleteLicenseAsync(model, dbContext, cancellationToken);

    public async Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user, CancellationToken cancellationToken)
        => await LicenseQuery.GetClaimLicenseUserAsync(user, dbContext, cancellationToken);

    public async Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user, CancellationToken cancellationToken)
        => await LicenseQuery.CheckUserLicenseExpiredAsync(user, dbContext, cancellationToken);
}