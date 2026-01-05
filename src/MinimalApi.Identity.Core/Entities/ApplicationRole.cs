using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class ApplicationRole : IdentityRole<int>, IEntity
{
    public ApplicationRole()
    { }

    public ApplicationRole(string roleName) : base(roleName)
    { }

    public bool Default { get; set; } = false;
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}