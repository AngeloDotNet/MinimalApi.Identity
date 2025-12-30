using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services;

namespace MinimalApi.Identity.API.Extensions;

public static class EndpointsHandler
{
    [AllowAnonymous]
    public static async Task<IResult> RegisterAsync([FromServices] IAuthService authService, [FromBody] RegisterModel inputModel)
    {
        var result = await authService.RegisterAsync(inputModel);
        return TypedResults.Ok(result);
    }

    [AllowAnonymous]
    public static async Task<IResult> LoginAsync([FromServices] IAuthService authService, [FromBody] LoginModel inputModel)
    {
        var result = await authService.LoginAsync(inputModel);
        return TypedResults.Ok(result);
    }

    [AllowAnonymous]
    public static async Task<IResult> RefreshTokenAsync([FromServices] IAuthService authService, [FromBody] RefreshTokenModel inputModel)
    {
        var result = await authService.RefreshTokenAsync(inputModel);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> ImpersonateUseAsync([FromServices] IAuthService authService, [FromBody] ImpersonateUserModel inputModel)
    {
        var result = await authService.ImpersonateAsync(inputModel);
        return TypedResults.Ok(result);
    }

    [AllowAnonymous]
    public static async Task<IResult> LogoutAsync([FromServices] IAuthService authService)
    {
        await authService.LogoutAsync();
        return TypedResults.Ok("User logged out successfully.");
    }

    public static async Task<IResult> ForgotPasswordAsync([FromServices] IAuthService authService, [FromBody] ForgotPasswordModel inputModel)
    {
        await authService.ForgotPasswordAsync(inputModel);
        return TypedResults.Ok("Password reset instructions have been sent to your email.");
    }

    public static async Task<IResult> ResetPasswordAsync([FromServices] IAuthService authService, [FromBody] ResetPasswordModel inputModel, [FromRoute] string code)
    {
        await authService.ResetPasswordAsync(inputModel, code);
        return TypedResults.Ok("Password has been reset successfully.");
    }
}