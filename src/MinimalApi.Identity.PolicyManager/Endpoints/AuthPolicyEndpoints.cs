using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Extensions;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Endpoints;

public static class AuthPolicyEndpoints
{
    public static IEndpointRouteBuilder MapPolicyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsAuthPolicyGroup)
            //.WithOpenApi(opt =>
            //{
            //    opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsAuthPolicyTag }];
            //    return opt;
            //});
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<Ok<List<PolicyResponseModel>>>(StatusCodes.Status200OK).WithDescription("Get all authorization policy")
            //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.AuthPolicyRead))
            //.WithOpenApi(opt =>
            //{
            //    opt.Description = "Get all authorization policy";
            //    opt.Summary = "Get all authorization policy";

            //    opt.Response(StatusCodes.Status200OK).Description = "Claims retrieved successfully";
            //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
            //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

            //    return opt;
            //})
            .WithDescription("Get all authorization policy")
            .WithSummary("Get all authorization policy");

        apiGroup.MapPost(PolicyExtensions.EndpointsCreateAuthPolicy, EndpointsHandler.CreateHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Add authorization policy")
            //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
            .WithValidation<CreatePolicyModel>()
            //.WithOpenApi(opt =>
            //{
            //    opt.Description = "Add authorization policy";
            //    opt.Summary = "Add authorization policy";

            //    opt.Response(StatusCodes.Status200OK).Description = "Policy added successfully";
            //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
            //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

            //    return opt;
            //});
            .WithDescription("Add authorization policy")
            .WithSummary("Add authorization policy");

        apiGroup.MapDelete(PolicyExtensions.EndpointsDeleteAuthPolicy, EndpointsHandler.DeleteHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription(ConstantsConfiguration.Unauthorized)
            //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .RequireAuthorization(nameof(Permissions.AuthPolicyWrite))
            .WithValidation<DeletePolicyModel>()
            //.WithOpenApi(opt =>
            //{
            //    opt.Description = "Delete authorization policy";
            //    opt.Summary = "Delete authorization policy";

            //    opt.Response(StatusCodes.Status200OK).Description = "Policy deleted successfully";
            //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
            //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

            //    return opt;
            //});
            .WithDescription("Delete authorization policy")
            .WithSummary("Delete authorization policy");

        return apiGroup;
    }
}
