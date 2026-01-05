using System.Security.Claims;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.ProfileManager.Services;

public interface IProfileService
{
    Task<List<UserProfileModel>> GetAllProfilesAsync(CancellationToken cancellationToken);
    Task<UserProfileModel> GetProfileAsync(int userId, CancellationToken cancellationToken);
    Task<string> CreateProfileAsync(CreateUserProfileModel model, CancellationToken cancellationToken);
    Task<string> EditProfileAsync(EditUserProfileModel model, CancellationToken cancellationToken);
    Task<List<Claim>> GetClaimUserProfileAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model, CancellationToken cancellationToken);
    Task<bool> UpdateLastDateChangePasswordAsync(int userId, CancellationToken cancellationToken);
}
