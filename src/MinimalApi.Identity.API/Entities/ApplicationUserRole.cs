using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class ApplicationUserRole : IdentityUserRole<int>, IEntity
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}