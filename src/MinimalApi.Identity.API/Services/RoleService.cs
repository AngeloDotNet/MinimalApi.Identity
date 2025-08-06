using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.API.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager) : IRoleService
{
    public async Task<List<RoleResponseModel>> GetAllRolesAsync()
    {
        var roles = await roleManager.Roles
            .Select(r => new RoleResponseModel(r.Id, r.Name!, r.Default))
            .ToListAsync();

        return roles.Count == 0 ? throw new NotFoundRoleException(MessagesAPI.RolesNotFound) : roles;
    }

    public async Task<string> CreateRoleAsync(CreateRoleModel model)
    {
        if (await roleManager.RoleExistsAsync(model.Role))
        {
            throw new ConflictException(MessagesAPI.RoleExists);
        }

        var newRole = new ApplicationRole
        {
            Name = model.Role,
            Default = false
        };

        var result = await roleManager.CreateAsync(newRole);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessagesAPI.RoleCreated;
    }

    public async Task<string> AssignRoleAsync(AssignRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username)
            ?? throw new NotFoundUserException(MessagesAPI.UserNotFound);

        if (!await roleManager.RoleExistsAsync(model.Role))
        {
            throw new NotFoundRoleException(MessagesAPI.RoleNotFound);
        }

        var result = await userManager.AddToRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessagesAPI.RoleAssigned;
    }

    public async Task<string> RevokeRoleAsync(RevokeRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username)
            ?? throw new NotFoundUserException(MessagesAPI.UserNotFound);

        var result = await userManager.RemoveFromRoleAsync(user, model.Role);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessagesAPI.RoleCanceled;
    }

    public async Task<string> DeleteRoleAsync(DeleteRoleModel model)
    {
        var role = await roleManager.FindByNameAsync(model.Role)
            ?? throw new NotFoundRoleException(MessagesAPI.RoleNotFound);

        if (role.Default)
        {
            throw new BadRequestRoleException(MessagesAPI.RoleNotDeleted);
        }

        var result = await roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            throw new BadRequestRoleException(result.Errors);
        }

        return MessagesAPI.RoleDeleted;
    }
}
