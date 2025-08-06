using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.API.Services;

public class ClaimsService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IClaimsService
{
    public async Task<List<ClaimResponseModel>> GetAllClaimsAsync()
    {
        var query = await dbContext.Set<ClaimType>().AsNoTracking().ToListAsync();

        return query.Count == 0 ? throw new NotFoundClaimException(MessagesAPI.ClaimsNotFound)
            : query.Select(c => new ClaimResponseModel(c.Id, c.Type, c.Value, c.Default)).ToList();
    }

    public async Task<string> CreateClaimAsync(CreateClaimModel model)
    {
        if (!CheckClaimTypeIsValid(model.Type))
        {
            throw new BadRequestException(MessagesAPI.ClaimTypeInvalid);
        }

        if (await CheckClaimExistAsync(model))
        {
            throw new ConflictException(MessagesAPI.ClaimAlreadyExist);
        }

        var claimType = new ClaimType
        {
            Type = model.Type,
            Value = model.Value,
            Default = false
        };

        dbContext.Set<ClaimType>().Add(claimType);
        await dbContext.SaveChangesAsync();

        return MessagesAPI.ClaimCreated;
    }

    public async Task<string> AssignClaimAsync(AssignClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundUserException(MessagesAPI.UserNotFound);

        var claim = await dbContext.Set<ClaimType>().AsNoTracking().FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotFoundClaimException(MessagesAPI.ClaimNotFound);

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            throw new BadRequestException(MessagesAPI.ClaimAlreadyAssigned);
        }

        var result = await userManager.AddClaimAsync(user, new Claim(model.Type, model.Value));

        return result.Succeeded ? MessagesAPI.ClaimAssigned : throw new BadRequestException(MessagesAPI.ClaimNotAssigned);
    }

    public async Task<string> RevokeClaimAsync(RevokeClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundUserException(MessagesAPI.UserNotFound);

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (!userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            throw new BadRequestException(MessagesAPI.ClaimNotAssigned);
        }

        var claimRemove = new Claim(model.Type, model.Value);
        var result = await userManager.RemoveClaimAsync(user, claimRemove);

        return result.Succeeded ? MessagesAPI.ClaimRevoked : throw new BadRequestException(MessagesAPI.ClaimNotRevoked);
    }

    public async Task<string> DeleteClaimAsync(DeleteClaimModel model)
    {
        var claim = await dbContext.Set<ClaimType>().FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotFoundClaimException(MessagesAPI.ClaimNotFound);

        if (claim.Default)
        {
            throw new BadRequestException(MessagesAPI.ClaimNotDeleted);
        }

        var isClaimAssigned = await dbContext.Users.AnyAsync(user
            => user.UserClaims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value));

        if (isClaimAssigned)
        {
            throw new BadRequestException(MessagesAPI.ClaimNotDeleted);
        }

        dbContext.Set<ClaimType>().Remove(claim);
        await dbContext.SaveChangesAsync();

        return MessagesAPI.ClaimDeleted;
    }

    private static bool CheckClaimTypeIsValid(string claimType)
        => !string.IsNullOrWhiteSpace(claimType) && Enum.TryParse<ClaimsType>(claimType, true, out _);

    private async Task<bool> CheckClaimExistAsync(CreateClaimModel model)
        => await dbContext.Set<ClaimType>().AnyAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase));
}
