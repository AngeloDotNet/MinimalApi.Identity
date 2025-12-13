using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Extensions;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Endpoints;

//public static class AuthPolicyEndpoints
public class AuthPolicyEndpoints : IEndpointRouteHandlerBuilder
{
    //public static IEndpointRouteBuilder MapPolicyEndpoints(this IEndpointRouteBuilder endpoints)
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsAuthPolicyGroup)
            .WithTags(EndpointGenerator.EndpointsAuthPolicyTag);

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<Ok<List<PolicyResponseModel>>>(StatusCodes.Status200OK).WithDescription("Policies retrieved successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.AuthPolicyRead))
            .WithDescription("Get all authorization policy")
            .WithSummary("Get all authorization policy");

        apiGroup.MapPost(PolicyExtensions.EndpointsCreateAuthPolicy, EndpointsHandler.CreateHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Policy added successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
            .WithValidation<CreatePolicyModel>()
            .WithDescription("Add authorization policy")
            .WithSummary("Add authorization policy");

        apiGroup.MapDelete(PolicyExtensions.EndpointsDeleteAuthPolicy, EndpointsHandler.DeleteHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Policy deleted successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
            .WithValidation<DeletePolicyModel>()
            .WithDescription("Delete authorization policy")
            .WithSummary("Delete authorization policy");

        //return apiGroup;
    }
}