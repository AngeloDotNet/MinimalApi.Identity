using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.ProfileManager.DependencyInjection;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.ProfileManager.Services;

public class ProfileService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task<List<UserProfileModel>> GetAllProfilesAsync(CancellationToken cancellationToken)
        => await ProfileQuery.GetAllProfilesAsync(dbContext, cancellationToken);

    public async Task<UserProfileModel> GetProfileAsync(int userId, CancellationToken cancellationToken)
        => await ProfileQuery.GetProfileAsync(userId, dbContext, userManager, cancellationToken);

    public async Task<string> CreateProfileAsync(CreateUserProfileModel model, CancellationToken cancellationToken)
        => await ProfileQuery.CreateProfileAsync(model, dbContext, cancellationToken);

    public async Task<string> EditProfileAsync(EditUserProfileModel model, CancellationToken cancellationToken)
        => await ProfileQuery.EditProfileAsync(model, dbContext, cancellationToken);

    public async Task<List<Claim>> GetClaimUserProfileAsync(ApplicationUser user, CancellationToken cancellationToken)
        => await ProfileQuery.GetClaimUserProfileAsync(user, dbContext, cancellationToken);

    public async Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model, CancellationToken cancellationToken)
        => await ProfileQuery.ChangeEnablementStatusUserProfileAsync(model, dbContext, cancellationToken);

    public async Task<bool> UpdateLastDateChangePasswordAsync(int userId, CancellationToken cancellationToken)
        => await ProfileQuery.UpdateLastDateChangePasswordAsync(userId, dbContext, cancellationToken);
}