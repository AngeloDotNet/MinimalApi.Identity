using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Extensions;

public static class EndpointsHandler
{
    #region "AuthEndpoints"

    [AllowAnonymous]
    public static async Task<IResult> RegisterAsync([FromServices] IAuthService authService, [FromBody] RegisterModel inputModel)
    {
        var result = await authService.RegisterAsync(inputModel);
        return Results.Ok(result);
    }

    [AllowAnonymous]
    public static async Task<IResult> LoginAsync([FromServices] IAuthService authService, [FromBody] LoginModel inputModel)
    {
        var result = await authService.LoginAsync(inputModel);
        return Results.Ok(result);
    }

    [AllowAnonymous]
    public static async Task<IResult> RefreshTokenAsync([FromServices] IAuthService authService, [FromBody] RefreshTokenModel inputModel)
    {
        var result = await authService.RefreshTokenAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> ImpersonateUseAsync([FromServices] IAuthService authService, [FromBody] ImpersonateUserModel inputModel)
    {
        var result = await authService.ImpersonateAsync(inputModel);
        return Results.Ok(result);
    }

    [AllowAnonymous]
    public static async Task<IResult> LogoutAsync([FromServices] IAuthService authService)
    {
        await authService.LogoutAsync();
        return Results.Ok("User logged out successfully.");
    }

    public static async Task<IResult> ForgotPasswordAsync([FromServices] IAuthService authService, [FromBody] ForgotPasswordModel inputModel)
    {
        await authService.ForgotPasswordAsync(inputModel);
        return Results.Ok("Password reset instructions have been sent to your email.");
    }

    public static async Task<IResult> ResetPasswordAsync([FromServices] IAuthService authService, [FromBody] ResetPasswordModel inputModel, [FromRoute] string code)
    {
        await authService.ResetPasswordAsync(inputModel, code);
        return Results.Ok("Password has been reset successfully.");
    }

    #endregion

    #region "ClaimsEndpoints"

    public static async Task<IResult> GetAllClaimsAsync([FromServices] IClaimsService claimsService)
    {
        var result = await claimsService.GetAllClaimsAsync();
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateClaimAsync([FromServices] IClaimsService claimsService, [FromBody] CreateClaimModel inputModel)
    {
        var result = await claimsService.CreateClaimAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> AssignClaimAsync([FromServices] IClaimsService claimsService, [FromBody] AssignClaimModel inputModel)
    {
        var result = await claimsService.AssignClaimAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> RevokeClaimAsync([FromServices] IClaimsService claimsService, [FromBody] RevokeClaimModel inputModel)
    {
        var result = await claimsService.RevokeClaimAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> DeleteClaimAsync([FromServices] IClaimsService claimsService, [FromBody] DeleteClaimModel inputModel)
    {
        var result = await claimsService.DeleteClaimAsync(inputModel);
        return Results.Ok(result);
    }

    #endregion

    #region "ModuleEndpoints"

    public static async Task<IResult> GetAllModulesAsync([FromServices] IModuleService moduleService)
    {
        var result = await moduleService.GetAllModulesAsync();
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateModuleAsync([FromServices] IModuleService moduleService, [FromBody] CreateModuleModel inputModel)
    {
        var result = await moduleService.CreateModuleAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> AssignModuleAsync([FromServices] IModuleService moduleService, [FromBody] AssignModuleModel inputModel)
    {
        var result = await moduleService.AssignModuleAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> RevokeModuleAsync([FromServices] IModuleService moduleService, [FromBody] RevokeModuleModel inputModel)
    {
        var result = await moduleService.RevokeModuleAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> DeleteModuleAsync([FromServices] IModuleService moduleService, [FromBody] DeleteModuleModel inputModel)
    {
        var result = await moduleService.DeleteModuleAsync(inputModel);
        return Results.Ok(result);
    }

    #endregion
}