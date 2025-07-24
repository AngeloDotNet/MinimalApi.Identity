using System.Security.Claims;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Licenses.Models;

namespace MinimalApi.Identity.Licenses.Services.Interfaces;

public interface ILicenseService
{
    Task<List<LicenseResponseModel>> GetAllLicensesAsync(CancellationToken cancellationToken);
    Task<string> CreateLicenseAsync(CreateLicenseModel model, CancellationToken cancellationToken);
    Task<string> AssignLicenseAsync(AssignLicenseModel model, CancellationToken cancellationToken);
    Task<string> RevokeLicenseAsync(RevokeLicenseModel model, CancellationToken cancellationToken);
    Task<string> DeleteLicenseAsync(DeleteLicenseModel model, CancellationToken cancellationToken);
    Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user);
    Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user);
}