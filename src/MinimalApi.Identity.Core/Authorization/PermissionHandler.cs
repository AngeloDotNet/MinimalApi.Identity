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
                var message = $"User {nameUser} not found";

                logger.LogWarning(message);
                throw new UserUnknownException(message);
            }

            if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
            {
                var message = ServiceCoreExtensions.UserLockedOut;

                logger.LogWarning(message);
                throw new UserIsLockedException(message);
            }

            if (securityStamp != utente.SecurityStamp)
            {
                var message = $"User {nameUser} security stamp is invalid";

                logger.LogWarning(message);
                throw new UserTokenIsInvalidException(message);
            }

            foreach (var permissionRequirement in permissionsRequirements)
            {
                if (permissionRequirement.Permissions.All(permission => user.HasClaim(ServiceCoreExtensions.Permission, permission)))
                {
                    context.Succeed(permissionRequirement);
                }
                else
                {
                    var message = $"User {nameUser} does not have the required permissions";

                    logger.LogWarning(message);
                    throw new UserWithoutPermissionsException(message);
                }
            }
        }

        await Task.CompletedTask;
    }
}