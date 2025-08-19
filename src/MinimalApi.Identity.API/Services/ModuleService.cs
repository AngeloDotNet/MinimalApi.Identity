using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.API.Services;

public class ModuleService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IModuleService
{
    public async Task<List<ModuleResponseModel>> GetAllModulesAsync()
    {
        var result = await dbContext.Set<Module>()
            .Select(m => new ModuleResponseModel(m.Id, m.Name, m.Description))
            .ToListAsync();

        return result.Count == 0 ? throw new NotFoundException(MessagesAPI.ModulesNotFound) : result;
    }

    public async Task<string> CreateModuleAsync(CreateModuleModel model)
    {
        if (await CheckModuleExistAsync(model))
        {
            throw new ConflictException(MessagesAPI.ModuleAlreadyExist);
        }

        var module = new Module
        {
            Name = model.Name,
            Description = model.Description
        };

        dbContext.Set<Module>().Add(module);
        await dbContext.SaveChangesAsync();

        return MessagesAPI.ModuleCreated;
    }

    public async Task<string> AssignModuleAsync(AssignModuleModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundException(MessagesAPI.UserNotFound);

        var module = await dbContext.Set<Module>().FindAsync(model.ModuleId)
            ?? throw new NotFoundException(MessagesAPI.ModuleNotFound);

        var userHasModule = await dbContext.Set<UserModule>()
            .AnyAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId);

        if (userHasModule)
        {
            throw new BadRequestException(MessagesAPI.ModuleNotAssignable);
        }

        var userModule = new UserModule
        {
            UserId = model.UserId,
            ModuleId = model.ModuleId
        };

        dbContext.Set<UserModule>().Add(userModule);
        await dbContext.SaveChangesAsync();

        return MessagesAPI.ModuleAssigned;
    }

    public async Task<string> RevokeModuleAsync(RevokeModuleModel model)
    {
        var userModule = await dbContext.Set<UserModule>()
            .SingleOrDefaultAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId)
            ?? throw new NotFoundException(MessagesAPI.ModuleNotFound);

        dbContext.Set<UserModule>().Remove(userModule);
        await dbContext.SaveChangesAsync();

        return MessagesAPI.ModuleCanceled;
    }

    public async Task<string> DeleteModuleAsync(DeleteModuleModel model)
    {
        var module = await dbContext.Set<Module>().FindAsync(model.ModuleId)
            ?? throw new NotFoundException(MessagesAPI.ModuleNotFound);

        if (await dbContext.Set<UserModule>().AnyAsync(ul => ul.ModuleId == model.ModuleId))
        {
            throw new BadRequestException(MessagesAPI.ModuleNotDeleted);
        }

        dbContext.Set<Module>().Remove(module);
        await dbContext.SaveChangesAsync();

        return MessagesAPI.ModuleDeleted;
    }

    public async Task<List<Claim>> GetClaimsModuleUserAsync(ApplicationUser user)
    {
        var result = await dbContext.Set<UserModule>()
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => ul.Module.Name)
            .ToListAsync();

        return result.Select(moduleName => new Claim(CustomClaimTypes.Module, moduleName)).ToList();
    }

    private async Task<bool> CheckModuleExistAsync(CreateModuleModel inputModel)
        => await dbContext.Set<Module>()
                                    .AsNoTracking()
                                    .AnyAsync(m => m.Name.Equals(inputModel.Name, StringComparison.InvariantCultureIgnoreCase));
}