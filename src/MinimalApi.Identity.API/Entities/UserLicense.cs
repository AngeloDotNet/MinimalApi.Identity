using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class UserLicense : IEntity
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public int LicenseId { get; set; }
    public License License { get; set; } = null!;
}