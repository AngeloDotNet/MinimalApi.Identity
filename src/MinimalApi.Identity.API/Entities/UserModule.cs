using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class UserModule : IEntity
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;
}