using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        return query.Count == 0 ? throw new NotFoundException(MessagesApi.ClaimsNotFound)
            : query.Select(c => new ClaimResponseModel(c.Id, c.Type, c.Value, c.Default)).ToList();
    }

    public async Task<string> CreateClaimAsync(CreateClaimModel model)
    {
        if (!CheckClaimTypeIsValid(model.Type))
        {
            throw new BadRequestException(MessagesApi.ClaimTypeInvalid);
        }

        if (await CheckClaimExistAsync(model))
        {
            throw new ConflictException(MessagesApi.ClaimAlreadyExist);
        }

        var claimType = new ClaimType
        {
            Type = model.Type,
            Value = model.Value,
            Default = false
        };

        dbContext.Set<ClaimType>().Add(claimType);
        await dbContext.SaveChangesAsync();

        return MessagesApi.ClaimCreated;
    }

    public async Task<string> AssignClaimAsync(AssignClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundException(MessagesApi.UserNotFound);

        var claim = await dbContext.Set<ClaimType>().AsNoTracking().FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotFoundException(MessagesApi.ClaimNotFound);

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            throw new BadRequestException(MessagesApi.ClaimAlreadyAssigned);
        }

        var result = await userManager.AddClaimAsync(user, new Claim(model.Type, model.Value));

        return result.Succeeded ? MessagesApi.ClaimAssigned : throw new BadRequestException(MessagesApi.ClaimNotAssigned);
    }

    public async Task<string> RevokeClaimAsync(RevokeClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundException(MessagesApi.UserNotFound);

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (!userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            throw new BadRequestException(MessagesApi.ClaimNotAssigned);
        }

        var claimRemove = new Claim(model.Type, model.Value);
        var result = await userManager.RemoveClaimAsync(user, claimRemove);

        return result.Succeeded ? MessagesApi.ClaimRevoked : throw new BadRequestException(MessagesApi.ClaimNotRevoked);
    }

    public async Task<string> DeleteClaimAsync(DeleteClaimModel model)
    {
        var claim = await dbContext.Set<ClaimType>().FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotFoundException(MessagesApi.ClaimNotFound);

        if (claim.Default)
        {
            throw new BadRequestException(MessagesApi.ClaimNotDeleted);
        }

        var isClaimAssigned = await dbContext.Users.AnyAsync(user
            => user.UserClaims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value));

        if (isClaimAssigned)
        {
            throw new BadRequestException(MessagesApi.ClaimNotDeleted);
        }

        dbContext.Set<ClaimType>().Remove(claim);
        await dbContext.SaveChangesAsync();

        return MessagesApi.ClaimDeleted;
    }

    private static bool CheckClaimTypeIsValid(string claimType)
        => !string.IsNullOrWhiteSpace(claimType) && Enum.TryParse<ClaimsType>(claimType, true, out _);

    private async Task<bool> CheckClaimExistAsync(CreateClaimModel model)
        => await dbContext.Set<ClaimType>().AnyAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase));
}