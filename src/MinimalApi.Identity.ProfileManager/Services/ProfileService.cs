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
    //{
    //    var profiles = await dbContext.Set<UserProfile>()
    //        .AsNoTracking()
    //        .Select(profile => ProfileMapper.ToEntity(profile))
    //        .ToListAsync(cancellationToken);

    //    //return profiles.Count == 0 ? throw new NotFoundProfileException(MessagesAPI.ProfilesNotFound) : profiles;
    //    return profiles.Count == 0 ? throw new NotFoundException(MessagesAPI.ProfilesNotFound) : profiles;
    //}

    public async Task<UserProfileModel> GetProfileAsync(int userId, CancellationToken cancellationToken)
        => await ProfileQuery.GetProfileAsync(userId, dbContext, userManager, cancellationToken);
    //{
    //    var user = await userManager.FindByIdAsync(userId.ToString())
    //        //?? throw new NotFoundProfileException(MessagesAPI.ProfileNotFound);
    //        ?? throw new NotFoundException(MessagesAPI.ProfileNotFound);

    //    var profile = await dbContext.Set<UserProfile>()
    //        .AsNoTracking()
    //        .Where(x => x.UserId == user.Id)
    //        .FirstOrDefaultAsync(cancellationToken)
    //        //?? throw new NotFoundProfileException(MessagesAPI.ProfileNotFound);
    //        ?? throw new NotFoundException(MessagesAPI.ProfileNotFound);

    //    return ProfileMapper.ToEntity(profile);
    //}

    public async Task<string> CreateProfileAsync(CreateUserProfileModel model, CancellationToken cancellationToken)
        => await ProfileQuery.CreateProfileAsync(model, dbContext, cancellationToken);
    //{
    //    var profile = new UserProfile(model.UserId, model.FirstName, model.LastName);

    //    profile.ChangeUserEnabled(true);
    //    profile.ChangeLastDateChangePassword(DateOnly.FromDateTime(DateTime.Now));

    //    dbContext.Set<UserProfile>().Add(profile);
    //    var result = await dbContext.SaveChangesAsync(cancellationToken);

    //    return result > 0 ? MessagesAPI.ProfileCreated : MessagesAPI.ProfileNotCreated;
    //}

    public async Task<string> EditProfileAsync(EditUserProfileModel model, CancellationToken cancellationToken)
        => await ProfileQuery.EditProfileAsync(model, dbContext, cancellationToken);
    //{
    //    var profile = await dbContext.Set<UserProfile>().AsNoTracking()
    //        .Where(x => x.UserId == model.UserId)
    //        .FirstOrDefaultAsync(cancellationToken)
    //        //?? throw new NotFoundProfileException(MessagesAPI.ProfileNotFound);
    //        ?? throw new NotFoundException(MessagesAPI.ProfileNotFound);

    //    profile.ChangeFirstName(model.FirstName);
    //    profile.ChangeLastName(model.LastName);

    //    dbContext.Set<UserProfile>().Update(profile);
    //    var result = await dbContext.SaveChangesAsync(cancellationToken);

    //    return result > 0 ? MessagesAPI.ProfileUpdated : throw new BadRequestException(MessagesAPI.ProfileNotUpdated);
    //}

    public async Task<List<Claim>> GetClaimUserProfileAsync(ApplicationUser user, CancellationToken cancellationToken)
        => await ProfileQuery.GetClaimUserProfileAsync(user, dbContext, cancellationToken);
    //{
    //    var result = await dbContext.Set<UserProfile>()
    //        .AsNoTracking()
    //        .Where(ul => ul.UserId == user.Id)
    //        .Select(ul => new { ul.FirstName, ul.LastName })
    //        .FirstOrDefaultAsync(cancellationToken);

    //    if (result == null)
    //    {
    //        return [];
    //    }

    //    return
    //        [
    //            new Claim(ClaimTypes.GivenName, result.FirstName ?? string.Empty),
    //            new Claim(ClaimTypes.Surname, result.LastName ?? string.Empty),
    //            new Claim(ProfileExtensions.FullName, $"{result.FirstName} {result.LastName}")
    //        ];
    //}

    public async Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model, CancellationToken cancellationToken)
        => await ProfileQuery.ChangeEnablementStatusUserProfileAsync(model, dbContext, cancellationToken);
    //{
    //    var profile = await dbContext.Set<UserProfile>()
    //        .AsNoTracking()
    //        .Where(x => x.UserId == model.UserId)
    //        .FirstOrDefaultAsync(cancellationToken)
    //        //?? throw new NotFoundProfileException(MessagesAPI.ProfileNotFound);
    //        ?? throw new NotFoundException(MessagesAPI.ProfileNotFound);

    //    profile.ChangeUserEnabled(model.IsEnabled);

    //    dbContext.Set<UserProfile>().Update(profile);
    //    var result = await dbContext.SaveChangesAsync(cancellationToken);

    //    return result > 0 ? model.IsEnabled ? MessagesAPI.ProfileEnabled : MessagesAPI.ProfileDisabled
    //        : throw new BadRequestException(model.IsEnabled ? MessagesAPI.ProfileNotEnabled : MessagesAPI.ProfileNotDisabled);
    //}
}