using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.PolicyManager.Models;
using MinimalApi.Identity.PolicyManager.Services;

namespace MinimalApi.Identity.PolicyManager.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> GetAllHandlerAsync([FromServices] IAuthPolicyService authPolicyService, CancellationToken cancellationToken)
    {
        var result = await authPolicyService.GetAllPoliciesAsync(cancellationToken);
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateHandlerAsync([FromServices] IAuthPolicyService authPolicyService,
        [FromBody] CreatePolicyModel inputModel, CancellationToken cancellationToken)
    {
        var result = await authPolicyService.CreatePolicyAsync(inputModel, cancellationToken);
        return Results.Ok(result);
    }

    public static async Task<IResult> DeleteHandlerAsync([FromServices] IAuthPolicyService authPolicyService,
        [FromBody] DeletePolicyModel inputModel, CancellationToken cancellationToken)
    {
        var result = await authPolicyService.DeletePolicyAsync(inputModel, cancellationToken);
        return Results.Ok(result);
    }
}