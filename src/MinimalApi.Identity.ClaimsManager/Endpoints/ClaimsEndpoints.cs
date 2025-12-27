using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.ClaimsManager.DependencyInjection;
using MinimalApi.Identity.ClaimsManager.Extensions;
using MinimalApi.Identity.ClaimsManager.Models;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Utility.Generators;

namespace MinimalApi.Identity.ClaimsManager.Endpoints;

public class ClaimsEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsClaimsGroup)
            .WithTags(EndpointGenerator.EndpointsClaimsTag)
            .RequireAuthorization();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllClaimsAsync)
            .Produces<Ok<List<ClaimResponseModel>>>(StatusCodes.Status200OK).WithDescription("Claims retrieved successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.ClaimRead))
            .WithDescription("Get all claims")
            .WithSummary("Get all claims");

        apiGroup.MapPost(ClaimExtensions.EndpointsCreateClaim, EndpointsHandler.CreateClaimAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim created successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ClaimWrite))
            .WithValidation<CreateClaimModel>()
            .WithDescription("Add claim")
            .WithSummary("Add claim");

        apiGroup.MapPost(ClaimExtensions.EndpointsAssignClaim, EndpointsHandler.AssignClaimAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim assigned successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ClaimWrite))
            .WithValidation<AssignClaimModel>()
            .WithDescription("Assign a claim to a user")
            .WithSummary("Assign a claim to a user");

        apiGroup.MapDelete(ClaimExtensions.EndpointsRevokeClaim, EndpointsHandler.RevokeClaimAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim revoked successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ClaimWrite))
            .WithValidation<RevokeClaimModel>()
            .WithDescription("Revoke a claim from a user")
            .WithSummary("Revoke a claim from a user");

        apiGroup.MapDelete(ClaimExtensions.EndpointsDeleteClaim, EndpointsHandler.DeleteClaimAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim deleted successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ClaimWrite))
            .WithValidation<DeleteClaimModel>()
            .WithDescription("Delete claim")
            .WithSummary("Delete claim");

        return apiGroup;
    }
}