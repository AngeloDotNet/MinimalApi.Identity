using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.ClaimsManager.Models;
using MinimalApi.Identity.ClaimsManager.Services;

namespace MinimalApi.Identity.ClaimsManager.Extensions;

public static class EndpointsHandler
{
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
}