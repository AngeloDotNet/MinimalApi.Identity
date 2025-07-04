﻿using System.Security.Claims;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IProfileService
{
    Task<List<UserProfileModel>> GetProfilesAsync();
    Task<UserProfileModel> GetProfileAsync(int userId);
    Task<string> CreateProfileAsync(CreateUserProfileModel model);
    Task<string> EditProfileAsync(EditUserProfileModel model);
    Task<List<Claim>> GetClaimUserProfileAsync(ApplicationUser user);
    Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model);
}
