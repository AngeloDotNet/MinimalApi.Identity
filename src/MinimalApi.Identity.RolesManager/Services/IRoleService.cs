using MinimalApi.Identity.RolesManager.Models;

namespace MinimalApi.Identity.RolesManager.Services;

public interface IRoleService
{
    Task<List<RoleResponseModel>> GetAllRolesAsync(CancellationToken cancellationToken);
    Task<string> CreateRoleAsync(CreateRoleModel model, CancellationToken cancellationToken);
    Task<string> AssignRoleAsync(AssignRoleModel model, CancellationToken cancellationToken);
    Task<string> RevokeRoleAsync(RevokeRoleModel model, CancellationToken cancellationToken);
    Task<string> DeleteRoleAsync(DeleteRoleModel model, CancellationToken cancellationToken);
}