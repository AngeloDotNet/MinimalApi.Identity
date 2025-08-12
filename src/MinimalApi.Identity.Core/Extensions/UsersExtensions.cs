using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.Core.Extensions;

public static class UsersExtensions
{
    public static string GetUserId(this IPrincipal user)
    {
        if (user is not ClaimsPrincipal claimsPrincipal)
        {
            throw new ArgumentException("User must be a ClaimsPrincipal", nameof(user));
        }

        var claimValue = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(claimValue))
        {
            throw new InvalidOperationException("Claim value for NameIdentifier is missing or empty.");
        }

        return claimValue;
    }

    public static string GetClaimValue(this IPrincipal user, string claimType)
        => ((ClaimsPrincipal)user).FindFirst(claimType)?.Value!;

    //public static bool IsAuthenticated(this ClaimsPrincipal principal)
    //{
    //    var userIsAuthenticated = principal.Identity?.IsAuthenticated ?? false;

    //    if (!userIsAuthenticated)
    //    {
    //        throw new UserUnknownException("User is not authenticated");
    //    }

    //    return userIsAuthenticated;
    //}

    public static async Task<bool> IsAuthValidAsync(this ClaimsPrincipal principal, UserManager<ApplicationUser> userManager)
    {
        var securityStamp = principal.GetClaimValue(ClaimTypes.SerialNumber);
        var userIsAuthenticated = principal.Identity?.IsAuthenticated ?? false;

        if (!userIsAuthenticated)
        {
            //throw new UserUnknownException("User is not authenticated");
            return false;
        }

        if (!SecurityStampIsValid(principal, securityStamp))
        {
            //throw new UserTokenIsInvalidException("User security stamp is invalid");
            return false;
        }

        if (!await UserIsLockedAsync(principal, userManager))
        {
            //throw new UserIsLockedException(MessagesAPI.UserLockedOut);
            return false;
        }

        return userIsAuthenticated;
    }

    public static ClaimsIdentity GetIdentity(this IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

    internal static bool SecurityStampIsValid(this ClaimsPrincipal user, string securityStamp)
    {
        //if (user == null)
        //{
        //    throw new ArgumentNullException(nameof(user), "User cannot be null");
        //}

        //if (string.IsNullOrEmpty(securityStamp))
        //{
        //    throw new ArgumentException("Security stamp cannot be null or empty", nameof(securityStamp));
        //}

        var userSecurityStamp = user.GetClaimValue(ClaimTypes.SerialNumber);

        //if (userSecurityStamp != securityStamp)
        if (user.GetClaimValue(ClaimTypes.SerialNumber) != securityStamp)
        {
            //throw new UserTokenIsInvalidException("User security stamp is invalid");
            return false;
        }

        return true;
    }

    internal static async Task<bool> UserIsLockedAsync(this ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
    {
        //if (user == null)
        //{
        //    throw new ArgumentNullException(nameof(user), "User cannot be null");
        //}

        var utente = await userManager.FindByIdAsync(user!.GetUserId());
        var lockoutEnd = utente!.LockoutEnd.GetValueOrDefault();

        //if (string.IsNullOrEmpty(lockoutEnd))
        //{
        //    return false;
        //}

        //if (DateTimeOffset.TryParse(lockoutEnd, out var lockoutEndDate))
        //{
        //    return lockoutEndDate > DateTimeOffset.UtcNow;
        //}

        if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
        {
            //logger.LogWarning(MessagesAPI.UserLockedOut);
            //throw new UserIsLockedException(MessagesAPI.UserLockedOut);
            return false;
        }

        //throw new InvalidOperationException("Invalid lockout end date format.");
        return true;
    }
}
