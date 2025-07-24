using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Extensions;

namespace MinimalApi.Identity.Core.Authorization;

public class PermissionHandler(ILogger<PermissionHandler> logger, UserManager<ApplicationUser> userManager) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var user = context.User;
        var permissionsRequirements = context.Requirements.OfType<PermissionRequirement>();

        if (UsersExtensions.IsAuthenticated(user))
        {
            var userId = user.GetUserId();
            var nameUser = user?.Identity?.Name;
            var securityStamp = context.User.GetClaimValue(ClaimTypes.SerialNumber);
            var utente = await userManager.FindByIdAsync(userId);

            if (utente == null || user == null)
            {
                logger.LogWarning($"User {nameUser} not found");
                throw new UserUnknownException($"User {nameUser} not found");
            }

            if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
            {
                logger.LogWarning(ServiceCoreExtensions.UserLockedOut);
                throw new UserIsLockedException(ServiceCoreExtensions.UserLockedOut);
            }

            if (securityStamp != utente.SecurityStamp)
            {
                logger.LogWarning($"User {nameUser} security stamp is invalid");
                throw new UserTokenIsInvalidException($"User {nameUser} security stamp is invalid");
            }

            foreach (var permissionRequirement in permissionsRequirements)
            {
                if (permissionRequirement.Permissions.All(permission => user.HasClaim(ServiceCoreExtensions.Permission, permission)))
                {
                    context.Succeed(permissionRequirement);
                }
                else
                {
                    logger.LogWarning($"User {nameUser} does not have the required permissions");
                    throw new UserWithoutPermissionsException($"User {nameUser} does not have the required permissions");
                }
            }
        }

        await Task.CompletedTask;
    }
}