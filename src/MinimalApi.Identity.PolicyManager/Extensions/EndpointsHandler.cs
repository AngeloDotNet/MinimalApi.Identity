using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.PolicyManager.Models;
using MinimalApi.Identity.PolicyManager.Services;

namespace MinimalApi.Identity.PolicyManager.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> GetAllHandlerAsync([FromServices] IAuthPolicyService authPolicyService, HttpContext httpContext)
    {
        var result = await authPolicyService.GetAllPoliciesAsync(httpContext.RequestAborted);
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateHandlerAsync([FromServices] IAuthPolicyService authPolicyService,
        [FromBody] CreatePolicyModel inputModel, HttpContext httpContext)
    {
        var result = await authPolicyService.CreatePolicyAsync(inputModel, httpContext.RequestAborted);
        return Results.Ok(result);
    }

    public static async Task<IResult> DeleteHandlerAsync([FromServices] IAuthPolicyService authPolicyService,
        [FromBody] DeletePolicyModel inputModel, HttpContext httpContext)
    {
        var result = await authPolicyService.DeletePolicyAsync(inputModel, httpContext.RequestAborted);
        return Results.Ok(result);
    }
}