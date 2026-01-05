using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.API.Mapping;

internal static class ProfileMapper
{
    internal static UserProfileModel ToEntity(this UserProfile userProfile)
    {
        return new UserProfileModel(userProfile.UserId, userProfile.FirstName, userProfile.LastName, userProfile.IsEnabled,
            userProfile.LastDateChangePassword);
    }
}