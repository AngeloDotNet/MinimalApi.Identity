using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.RolesManager.Models;

namespace MinimalApi.Identity.RolesManager.DependencyInjection;

public static class RolesQuery
{
    public static async Task<List<RoleResponseModel>> GetAllRolesAsync(RoleManager<ApplicationRole> roleManager, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles
            .Select(r => new RoleResponseModel(r.Id, r.Name!, r.Default))
            .ToListAsync().ConfigureAwait(false);

        return roles.Count == 0 ? throw new NotFoundException(MessagesApi.RoleNotFound) : roles;
    }

    public static async Task<string> CreateRoleAsync(RoleManager<ApplicationRole> roleManager, CreateRoleModel model)
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

    public static async Task<string> AssignRoleAsync(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, AssignRoleModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username)
            .ConfigureAwait(false) ?? throw new NotFoundException(MessagesApi.UserNotFound);

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

    public static async Task<string> RevokeRoleAsync(UserManager<ApplicationUser> userManager, RevokeRoleModel model)
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

    public static async Task<string> DeleteRoleAsync(RoleManager<ApplicationRole> roleManager, DeleteRoleModel model)
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