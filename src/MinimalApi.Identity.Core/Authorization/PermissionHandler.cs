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

        if (await UsersExtensions.IsAuthValidAsync(user, userManager))
        {
            //var userId = user.GetUserId();
            var nameUser = user?.Identity?.Name;
            //var securityStamp = context.User.GetClaimValue(ClaimTypes.SerialNumber);
            //var utente = await userManager.FindByIdAsync(userId);
            var utente = await userManager.FindByIdAsync(user!.GetUserId());

            if (utente == null || user == null)
            {
                logger.LogWarning("User {nameUser} not found", nameUser);
                throw new UserUnknownException($"User {nameUser} not found");
            }

            //if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
            //{
            //    logger.LogWarning(MessagesAPI.UserLockedOut);
            //    throw new UserIsLockedException(MessagesAPI.UserLockedOut);
            //}

            //if (securityStamp != utente.SecurityStamp)
            //{
            //    logger.LogWarning("User {nameUser} security stamp is invalid", nameUser);
            //    throw new UserTokenIsInvalidException($"User {nameUser} security stamp is invalid");
            //}

            //if (!user.SecurityStampIsValid(securityStamp))
            //{
            //    logger.LogWarning("User {nameUser} security stamp is invalid", nameUser);
            //    throw new UserTokenIsInvalidException($"User {nameUser} security stamp is invalid");
            //}

            foreach (var permissionRequirement in permissionsRequirements)
            {
                if (permissionRequirement.Permissions.All(permission => user.HasClaim(ServiceCoreExtensions.Permission, permission)))
                {
                    context.Succeed(permissionRequirement);
                }
                else
                {
                    logger.LogWarning("User {nameUser} does not have the required permissions", nameUser);
                    throw new UserWithoutPermissionsException($"User {nameUser} does not have the required permissions");
                }
            }
        }

        await Task.CompletedTask;
    }
}