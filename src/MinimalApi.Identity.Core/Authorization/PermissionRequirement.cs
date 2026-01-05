using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.Identity.Core.Authorization;

public class PermissionRequirement(params string[] permissions) : IAuthorizationRequirement
{
    public string[] Permissions { get; } = permissions;
}