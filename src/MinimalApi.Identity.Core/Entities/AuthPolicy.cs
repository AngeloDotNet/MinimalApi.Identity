using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class AuthPolicy : BaseEntity, IEntity
{
    public string PolicyName { get; set; } = null!;
    public string PolicyDescription { get; set; } = null!;
    public string[] PolicyPermissions { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}