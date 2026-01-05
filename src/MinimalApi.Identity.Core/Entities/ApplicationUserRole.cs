using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class ApplicationUserRole : IdentityUserRole<int>, IEntity
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}