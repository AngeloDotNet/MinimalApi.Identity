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
        ArgumentNullException.ThrowIfNull(user);

        if (user is not ClaimsPrincipal claimsPrincipal)
        {
            throw new ArgumentException("User must be a ClaimsPrincipal", nameof(user));
        }

        var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null || string.IsNullOrEmpty(claim.Value))
        {
            throw new InvalidOperationException("Claim value for NameIdentifier is missing or empty.");
        }

        return claim.Value;
    }

    public static string GetClaimValue(this IPrincipal user, string claimType)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claimType);

        if (user is not ClaimsPrincipal claimsPrincipal)
        {
            throw new ArgumentException("User must be a ClaimsPrincipal", nameof(user));
        }

        var claim = claimsPrincipal.FindFirst(claimType);
        return claim?.Value ?? throw new InvalidOperationException($"Claim value for {claimType} is missing.");
    }

    public static async Task<bool> IsAuthValidAsync(this ClaimsPrincipal principal, UserManager<ApplicationUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(userManager);

        var userIsAuthenticated = principal.Identity?.IsAuthenticated == true;

        if (!userIsAuthenticated)
        {
            return false;
        }

        var securityStamp = principal.GetClaimValue(ClaimTypes.SerialNumber);

        if (!SecurityStampIsValid(principal, securityStamp))
        {
            return false;
        }

        if (!await UserIsLockedAsync(principal, userManager).ConfigureAwait(false))
        {
            return false;
        }

        return true;
    }

    public static ClaimsIdentity GetIdentity(this IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        return httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
    }

    internal static bool SecurityStampIsValid(this ClaimsPrincipal user, string securityStamp)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(securityStamp);

        var userSecurityStamp = user.GetClaimValue(ClaimTypes.SerialNumber);
        return userSecurityStamp == securityStamp;
    }

    internal static async Task<bool> UserIsLockedAsync(this ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(userManager);

        var userId = user.GetUserId();
        var utente = await userManager.FindByIdAsync(userId).ConfigureAwait(false);

        if (utente is null)
        {
            return false;
        }

        var lockoutEnd = utente.LockoutEnd.GetValueOrDefault();
        return lockoutEnd <= DateTimeOffset.UtcNow;
    }
}

//public static class UsersExtensions
//{
//    public static string GetUserId(this IPrincipal user)
//    {
//        if (user is not ClaimsPrincipal claimsPrincipal)
//        {
//            throw new ArgumentException("User must be a ClaimsPrincipal", nameof(user));
//        }

//        var claimValue = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//        if (string.IsNullOrEmpty(claimValue))
//        {
//            throw new InvalidOperationException("Claim value for NameIdentifier is missing or empty.");
//        }

//        return claimValue;
//    }

//    public static string GetClaimValue(this IPrincipal user, string claimType)
//        => ((ClaimsPrincipal)user).FindFirst(claimType)?.Value!;

//    public static async Task<bool> IsAuthValidAsync(this ClaimsPrincipal principal, UserManager<ApplicationUser> userManager)
//    {
//        var securityStamp = principal.GetClaimValue(ClaimTypes.SerialNumber);
//        var userIsAuthenticated = principal.Identity?.IsAuthenticated ?? false;

//        if (!userIsAuthenticated || !SecurityStampIsValid(principal, securityStamp) || !await UserIsLockedAsync(principal, userManager))
//        {
//            return false;
//        }

//        return userIsAuthenticated;
//    }

//    public static ClaimsIdentity GetIdentity(this IHttpContextAccessor httpContextAccessor)
//        => httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

//    internal static bool SecurityStampIsValid(this ClaimsPrincipal user, string securityStamp)
//    {
//        var userSecurityStamp = user.GetClaimValue(ClaimTypes.SerialNumber);

//        if (user.GetClaimValue(ClaimTypes.SerialNumber) != securityStamp)
//        {
//            return false;
//        }

//        return true;
//    }

//    internal static async Task<bool> UserIsLockedAsync(this ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
//    {
//        var utente = await userManager.FindByIdAsync(user!.GetUserId());
//        var lockoutEnd = utente!.LockoutEnd.GetValueOrDefault();

//        if (utente.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
//        {
//            return false;
//        }

//        return true;
//    }
//}