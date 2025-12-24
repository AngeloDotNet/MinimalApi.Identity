using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.ModuleManager.DependencyInjection;
using MinimalApi.Identity.ModuleManager.Models;

namespace MinimalApi.Identity.ModuleManager.Services;

public class ModuleService(MinimalApiAuthDbContext dbContext) : IModuleService
{
    public async Task<List<ModuleResponseModel>> GetAllModulesAsync()
    {
        var result = await dbContext.Set<Module>()
            .Select(m => new ModuleResponseModel(m.Id, m.Name, m.Description))
            .ToListAsync().ConfigureAwait(false);

        if (result.Count == 0)
        {
            throw new NotFoundException(MessagesApi.ModulesNotFound);
        }

        return result;
    }

    public async Task<string> CreateModuleAsync(CreateModuleModel model)
    {
        if (await CheckModuleExistAsync(model).ConfigureAwait(false))
        {
            throw new ConflictException(MessagesApi.ModuleAlreadyExist);
        }

        var module = new Module
        {
            Name = model.Name,
            Description = model.Description
        };

        dbContext.Set<Module>().Add(module);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return MessagesApi.ModuleCreated;
    }

    public async Task<string> AssignModuleAsync(AssignModuleModel model)
    {
        var userHasModuleTask = await dbContext.Set<UserModule>()
            .AnyAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId);

        if (userHasModuleTask)
        {
            throw new BadRequestException(MessagesApi.ModuleNotAssignable);
        }

        var userModule = new UserModule
        {
            UserId = model.UserId,
            ModuleId = model.ModuleId
        };

        dbContext.Set<UserModule>().Add(userModule);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return MessagesApi.ModuleAssigned;
    }

    public async Task<string> RevokeModuleAsync(RevokeModuleModel model)
    {
        var userModule = await dbContext.Set<UserModule>()
            .SingleOrDefaultAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId)
            .ConfigureAwait(false) ?? throw new NotFoundException(MessagesApi.ModuleNotFound);

        dbContext.Set<UserModule>().Remove(userModule);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return MessagesApi.ModuleCanceled;
    }

    public async Task<string> DeleteModuleAsync(DeleteModuleModel model)
    {
        var module = await dbContext.Set<Module>().FindAsync(model.ModuleId).AsTask()
            .ConfigureAwait(false) ?? throw new NotFoundException(MessagesApi.ModuleNotFound);

        if (await dbContext.Set<UserModule>().AnyAsync(ul => ul.ModuleId == model.ModuleId).ConfigureAwait(false))
        {
            throw new BadRequestException(MessagesApi.ModuleNotDeleted);
        }

        dbContext.Set<Module>().Remove(module);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return MessagesApi.ModuleDeleted;
    }

    public async Task<List<Claim>> GetClaimsModuleUserAsync(ApplicationUser user)
    {
        var result = await dbContext.Set<UserModule>()
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => ul.Module.Name)
            .ToListAsync().ConfigureAwait(false);

        var claims = new List<Claim>(result.Count);

        foreach (var moduleName in result)
        {
            claims.Add(new Claim(ModuliExtensions.Module, moduleName));
        }

        return claims;
    }

    private async Task<bool> CheckModuleExistAsync(CreateModuleModel inputModel)
        => await dbContext.Set<Module>()
        .AnyAsync(m => m.Name.Equals(inputModel.Name, StringComparison.InvariantCultureIgnoreCase))
        .ConfigureAwait(false);
}