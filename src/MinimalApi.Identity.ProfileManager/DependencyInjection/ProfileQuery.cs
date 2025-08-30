using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Mapping;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.ProfileManager.DependencyInjection;

public static class ProfileQuery
{
    public static async Task<List<UserProfileModel>> GetAllProfilesAsync(MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var profiles = await dbContext.Set<UserProfile>()
            .AsNoTracking()
            .Select(profile => ProfileMapper.ToEntity(profile))
            .ToListAsync(cancellationToken);

        return profiles.Count == 0 ? throw new NotFoundException(MessagesApi.ProfilesNotFound) : profiles;
    }

    public static async Task<UserProfileModel> GetProfileAsync(int userId, MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException(MessagesApi.ProfileNotFound);

        var profile = await dbContext.Set<UserProfile>()
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.ProfileNotFound);

        return ProfileMapper.ToEntity(profile);
    }

    public static async Task<string> CreateProfileAsync(CreateUserProfileModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        //var profile = new UserProfile(model.UserId, model.FirstName, model.LastName);

        //profile.ChangeUserEnabled(true);
        //profile.ChangeLastDateChangePassword(DateOnly.FromDateTime(DateTime.Now));

        var profile = new UserProfile
        {
            UserId = model.UserId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            IsEnabled = true,
            LastDateChangePassword = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        dbContext.Set<UserProfile>().Add(profile);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        return result > 0 ? MessagesApi.ProfileCreated : MessagesApi.ProfileNotCreated;
    }

    public static async Task<string> EditProfileAsync(EditUserProfileModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var profile = await dbContext.Set<UserProfile>().AsNoTracking()
            .Where(x => x.UserId == model.UserId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.ProfileNotFound);

        profile.FirstName = model.FirstName;
        profile.LastName = model.LastName;

        //profile.ChangeFirstName(model.FirstName);
        //profile.ChangeLastName(model.LastName);

        dbContext.Set<UserProfile>().Update(profile);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        return result > 0 ? MessagesApi.ProfileUpdated : throw new BadRequestException(MessagesApi.ProfileNotUpdated);
    }

    public static async Task<List<Claim>> GetClaimUserProfileAsync(ApplicationUser user, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var result = await dbContext.Set<UserProfile>()
            .AsNoTracking()
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => new { ul.FirstName, ul.LastName })
            .FirstOrDefaultAsync(cancellationToken);

        return result switch
        {
            null => [],
            _ => [
                new Claim(ClaimTypes.GivenName, result.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, result.LastName ?? string.Empty),
                new Claim(ProfileExtensions.FullName, $"{result.FirstName} {result.LastName}")
            ]
        };
    }

    public static async Task<string> ChangeEnablementStatusUserProfileAsync(ChangeEnableProfileModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var profile = await dbContext.Set<UserProfile>()
            .AsNoTracking()
            .Where(x => x.UserId == model.UserId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.ProfileNotFound);

        //profile.ChangeUserEnabled(model.IsEnabled);
        profile.IsEnabled = model.IsEnabled;

        dbContext.Set<UserProfile>().Update(profile);
        var result = await dbContext.SaveChangesAsync(cancellationToken);

        return result > 0 ? model.IsEnabled ? MessagesApi.ProfileEnabled : MessagesApi.ProfileDisabled
            : throw new BadRequestException(model.IsEnabled ? MessagesApi.ProfileNotEnabled : MessagesApi.ProfileNotDisabled);
    }
}
