using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.API.Mapping;

internal static class ProfileMapper
{
    internal static UserProfileModel ToEntity(this UserProfile userProfile)
    {
        return new UserProfileModel(
            userProfile.UserId,
            userProfile.User.Email!,
            userProfile.FirstName,
            userProfile.LastName,
            userProfile.IsEnabled,
            userProfile.LastDateChangePassword
        );
    }

    //public static AuthPolicy ToEntityModel(this PolicyDetailsResponseModel model)
    //{
    //    return new AuthPolicy
    //    {
    //        PolicyName = model.PolicyName,
    //        PolicyDescription = model.PolicyDescription,
    //        PolicyPermissions = model.PolicyPermissions,
    //        IsDefault = model.IsDefault,
    //        IsActive = model.IsActive
    //    };
    //}
}
