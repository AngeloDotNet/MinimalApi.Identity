using System.Security.Claims;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Licenses.Models;

namespace MinimalApi.Identity.Licenses.Services.Interfaces;

public interface ILicenseService
{
    Task<List<LicenseResponseModel>> GetAllLicensesAsync();
    Task<string> CreateLicenseAsync(CreateLicenseModel model);
    Task<string> AssignLicenseAsync(AssignLicenseModel model);
    Task<string> RevokeLicenseAsync(RevokeLicenseModel model);
    Task<string> DeleteLicenseAsync(DeleteLicenseModel model);
    Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user);
    Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user);
}