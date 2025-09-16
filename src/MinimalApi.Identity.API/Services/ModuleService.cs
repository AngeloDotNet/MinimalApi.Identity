using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Configurations;
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
            .AsNoTracking().Select(m => new ModuleResponseModel(m.Id, m.Name, m.Description))
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
        var userTask = userManager.FindByIdAsync(model.UserId.ToString());
        var moduleTask = dbContext.Set<Module>().FindAsync(model.ModuleId).AsTask();
        var userHasModuleTask = dbContext.Set<UserModule>()
            .AnyAsync(um => um.UserId == model.UserId && um.ModuleId == model.ModuleId);

        await Task.WhenAll(userTask, moduleTask, userHasModuleTask).ConfigureAwait(false);

        var user = userTask.Result ?? throw new NotFoundException(MessagesApi.UserNotFound);
        var module = moduleTask.Result ?? throw new NotFoundException(MessagesApi.ModuleNotFound);

        if (userHasModuleTask.Result)
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
            .ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.ModuleNotFound);

        dbContext.Set<UserModule>().Remove(userModule);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return MessagesApi.ModuleCanceled;
    }

    public async Task<string> DeleteModuleAsync(DeleteModuleModel model)
    {
        var module = await dbContext.Set<Module>().FindAsync(model.ModuleId).AsTask().ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.ModuleNotFound);

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
            .AsNoTracking().Where(ul => ul.UserId == user.Id)
            .Select(ul => ul.Module.Name).ToListAsync()
            .ConfigureAwait(false);

        var claims = new List<Claim>(result.Count);

        foreach (var moduleName in result)
        {
            claims.Add(new Claim(ConstantsConfiguration.Module, moduleName));
        }

        return claims;
    }

    private async Task<bool> CheckModuleExistAsync(CreateModuleModel inputModel)
        => await dbContext.Set<Module>()
            .AsNoTracking().AnyAsync(m => m.Name.Equals(inputModel.Name, StringComparison.InvariantCultureIgnoreCase))
            .ConfigureAwait(false);
}