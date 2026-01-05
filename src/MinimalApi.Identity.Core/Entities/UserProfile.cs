using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class UserProfile : BaseEntity, IEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public DateOnly? LastDateChangePassword { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}