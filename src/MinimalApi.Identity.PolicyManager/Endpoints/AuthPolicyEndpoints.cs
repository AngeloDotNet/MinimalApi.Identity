using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Models;
using MinimalApi.Identity.PolicyManager.Services.Interfaces;

namespace MinimalApi.Identity.PolicyManager.Endpoints;

public static class AuthPolicyEndpoints
{
    public static IEndpointRouteBuilder MapPolicyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(PolicyExtensions.EndpointsAuthPolicyGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = PolicyExtensions.EndpointsAuthPolicyTag }];
                return opt;
            });

        apiGroup.MapGet(PolicyExtensions.EndpointsStringEmpty, async ([FromServices] IAuthPolicyService authPolicyService,
            HttpContext httpContext) =>
        {
            return await authPolicyService.GetAllPoliciesAsync(httpContext.RequestAborted);
        })
        .Produces<Ok<List<PolicyResponseModel>>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .RequireAuthorization(nameof(Permissions.AuthPolicyRead))
        .WithOpenApi(opt =>
        {
            opt.Description = "Get all authorization policy";
            opt.Summary = "Get all authorization policy";

            opt.Response(StatusCodes.Status200OK).Description = "Claims retrieved successfully";
            opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
            opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

            return opt;
        });

        apiGroup.MapPost(PolicyExtensions.EndpointsCreateAuthPolicy, async ([FromServices] IAuthPolicyService authPolicyService,
            [FromBody] CreatePolicyModel inputModel, HttpContext httpContext) =>
        {
            return await authPolicyService.CreatePolicyAsync(inputModel, httpContext.RequestAborted);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
        .WithValidation<CreatePolicyModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Add authorization policy";
            opt.Summary = "Add authorization policy";

            opt.Response(StatusCodes.Status200OK).Description = "Policy added successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
            opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

            return opt;
        });

        apiGroup.MapDelete(PolicyExtensions.EndpointsDeleteAuthPolicy, async ([FromServices] IAuthPolicyService authPolicyService,
            [FromBody] DeletePolicyModel inputModel, HttpContext httpContext) =>
        {
            return await authPolicyService.DeletePolicyAsync(inputModel, httpContext.RequestAborted);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
        .WithValidation<DeletePolicyModel>()
        .WithOpenApi(opt =>
        {
            opt.Description = "Delete authorization policy";
            opt.Summary = "Delete authorization policy";

            opt.Response(StatusCodes.Status200OK).Description = "Policy deleted successfully";
            opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
            opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

            return opt;
        });

        return apiGroup;
    }
}
