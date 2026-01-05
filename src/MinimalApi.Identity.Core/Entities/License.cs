using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class License : BaseEntity, IEntity
{
    public string Name { get; set; } = null!;
    public DateOnly ExpirationDate { get; set; }
    public ICollection<UserLicense> UserLicenses { get; set; } = [];
}