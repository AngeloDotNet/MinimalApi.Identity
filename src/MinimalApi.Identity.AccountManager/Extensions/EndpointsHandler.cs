using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.AccountManager.Models;
using MinimalApi.Identity.AccountManager.Services;

namespace MinimalApi.Identity.AccountManager.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> ConfirmEmailAsync([FromServices] IAccountService accountService, [FromBody] ConfirmEmailModel request)
    {
        var result = await accountService.ConfirmEmailAsync(request);
        return Results.Ok(result);
    }

    public static async Task<IResult> ChangeEmailAsync([FromServices] IAccountService accountService, [FromBody] ChangeEmailModel request)
    {
        var result = await accountService.ChangeEmailAsync(request);
        return Results.Ok(result);
    }

    public static async Task<IResult> ConfirmEmailChangeAsync([FromServices] IAccountService accountService, [FromBody] ConfirmEmailChangeModel request)
    {
        var result = await accountService.ConfirmEmailChangeAsync(request);
        return Results.Ok(result);
    }
}