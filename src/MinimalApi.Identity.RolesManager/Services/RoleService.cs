using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.RolesManager.DependencyInjection;
using MinimalApi.Identity.RolesManager.Models;

namespace MinimalApi.Identity.RolesManager.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager) : IRoleService
{
    public async Task<List<RoleResponseModel>> GetAllRolesAsync(CancellationToken cancellationToken)
        => await RolesQuery.GetAllRolesAsync(roleManager, cancellationToken);

    public async Task<string> CreateRoleAsync(CreateRoleModel model, CancellationToken cancellationToken)
        => await RolesQuery.CreateRoleAsync(roleManager, model);

    public async Task<string> AssignRoleAsync(AssignRoleModel model, CancellationToken cancellationToken)
        => await RolesQuery.AssignRoleAsync(roleManager, userManager, model);

    public async Task<string> RevokeRoleAsync(RevokeRoleModel model, CancellationToken cancellationToken)
        => await RolesQuery.RevokeRoleAsync(userManager, model);

    public async Task<string> DeleteRoleAsync(DeleteRoleModel model, CancellationToken cancellationToken)
        => await RolesQuery.DeleteRoleAsync(roleManager, model);
}