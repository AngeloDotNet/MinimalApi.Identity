using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            .ToListAsync().ConfigureAwait(false);

        return roles;
    }

    public async Task<string> CreateRoleAsync(CreateRoleModel model)
    {
        if (await roleManager.RoleExistsAsync(model.Role).ConfigureAwait(false))
        {
            throw new ConflictException(MessagesApi.RoleExists);
        }

        var newRole = new ApplicationRole
        {
            Name = model.Role,
            Default = false
        };

        var result = await roleManager.CreateAsync(newRole).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result.Errors);
        }

        return MessagesApi.RoleCreated;
    }

    public async Task<string> AssignRoleAsync(AssignRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username).ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.UserNotFound);

        if (!await roleManager.RoleExistsAsync(model.Role).ConfigureAwait(false))
        {
            throw new NotFoundException(MessagesApi.RoleNotFound);
        }

        var result = await userManager.AddToRoleAsync(user, model.Role).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result.Errors);
        }

        return MessagesApi.RoleAssigned;
    }

    public async Task<string> RevokeRoleAsync(RevokeRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username).ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.UserNotFound);

        var result = await userManager.RemoveFromRoleAsync(user, model.Role).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result.Errors);
        }

        return MessagesApi.RoleCanceled;
    }

    public async Task<string> DeleteRoleAsync(DeleteRoleModel model)
    {
        var role = await roleManager.FindByNameAsync(model.Role).ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.RoleNotFound);

        if (role.Default)
        {
            throw new BadRequestException(MessagesApi.RoleNotDeleted);
        }

        var result = await roleManager.DeleteAsync(role).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result.Errors);
        }

        return MessagesApi.RoleDeleted;
    }
}