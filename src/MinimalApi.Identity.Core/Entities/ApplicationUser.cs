using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class ApplicationUser : IdentityUser<int>, IEntity
{
    public string RefreshToken { get; set; } = null!;
    public DateTime? RefreshTokenExpirationDate { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
    public ICollection<IdentityUserClaim<int>> UserClaims { get; set; } = [];
    public ICollection<UserLicense> UserLicenses { get; set; } = [];
    public ICollection<UserModule> UserModules { get; set; } = [];
}