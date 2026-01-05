using System.Security.Claims;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.ModuleManager.Models;

namespace MinimalApi.Identity.ModuleManager.Services;

public interface IModuleService
{
    Task<List<ModuleResponseModel>> GetAllModulesAsync();
    Task<string> CreateModuleAsync(CreateModuleModel model);
    Task<string> AssignModuleAsync(AssignModuleModel model);
    Task<string> RevokeModuleAsync(RevokeModuleModel model);
    Task<string> DeleteModuleAsync(DeleteModuleModel model);
    Task<List<Claim>> GetClaimsModuleUserAsync(ApplicationUser user);
}