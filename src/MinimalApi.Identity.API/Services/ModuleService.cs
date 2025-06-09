using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Exceptions.Conflict;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.API.Services;

public class ModuleService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IModuleService
{
    public async Task<List<ModuleResponseModel>> GetAllModulesAsync()
    {
        var result = await dbContext.Set<Module>()
            .Select(m => new ModuleResponseModel(m.Id, m.Name, m.Description))
            .ToListAsync();

        //if (result.Count == 0)
        //{
        //    throw new NotFoundModuleException(MessageApi.ModulesNotFound);
        //}

        //return result;

        return result.Count == 0 ? throw new NotFoundModuleException(MessageApi.ModulesNotFound) : result;
    }

    public async Task<string> CreateModuleAsync(CreateModuleModel model)
    {
        if (await CheckModuleExistAsync(model))
        {
            throw new ConflictModuleException(MessageApi.ModuleAlreadyExist);
        }

        var module = new Module
        {
            Name = model.Name,
            Description = model.Description
        };

        dbContext.Set<Module>().Add(module);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleCreated;
    }

    public async Task<string> AssignModuleAsync(AssignModuleModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundUserException(MessageApi.UserNotFound);

        var module = await dbContext.Set<Module>().FindAsync(model.ModuleId)
            ?? throw new NotFoundModuleException(MessageApi.ModuleNotFound);

        var userHasModule = await dbContext.Set<UserModule>()
            .AnyAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId);

        if (userHasModule)
        {
            throw new BadRequestModuleException(MessageApi.ModuleNotAssignable);
        }

        var userModule = new UserModule
        {
            UserId = model.UserId,
            ModuleId = model.ModuleId
        };

        dbContext.Set<UserModule>().Add(userModule);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleAssigned;
    }

    public async Task<string> RevokeModuleAsync(RevokeModuleModel model)
    {
        var userModule = await dbContext.Set<UserModule>()
            .SingleOrDefaultAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId)
            ?? throw new NotFoundModuleException(MessageApi.ModuleNotFound);

        dbContext.Set<UserModule>().Remove(userModule);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleCanceled;
    }

    public async Task<string> DeleteModuleAsync(DeleteModuleModel model)
    {
        var module = await dbContext.Set<Module>().FindAsync(model.ModuleId)
            ?? throw new NotFoundModuleException(MessageApi.ModuleNotFound);

        if (await dbContext.Set<UserModule>().AnyAsync(ul => ul.ModuleId == model.ModuleId))
        {
            throw new BadRequestModuleException(MessageApi.ModuleNotDeleted);
        }

        dbContext.Set<Module>().Remove(module);
        await dbContext.SaveChangesAsync();

        return MessageApi.ModuleDeleted;
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
    {
        return await dbContext.Set<Module>().AnyAsync(m => m.Name.Equals(inputModel.Name, StringComparison.InvariantCultureIgnoreCase));
    }
}
