using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Utility.Generators;

namespace MinimalApi.Identity.API.Endpoints;

public class ClaimsEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsClaimsGroup)
            .RequireAuthorization()
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, async ([FromServices] IClaimsService claimsService) =>
        {
            return await claimsService.GetAllClaimsAsync();
        })
        .Produces<Ok<List<ClaimResponseModel>>>(StatusCodes.Status200OK).WithDescription("Claims retrieved successfully")
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .RequireAuthorization(nameof(Permissions.ClaimRead))
        .WithDescription("Get all claims")
        .WithSummary("Get all claims");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsCreateClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] CreateClaimModel inputModel) =>
        {
            return await claimsService.CreateClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim created successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<CreateClaimModel>()
        .WithDescription("Add claim")
        .WithSummary("Add claim");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAssignClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] AssignClaimModel inputModel) =>
        {
            return await claimsService.AssignClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim assigned successfully")
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<AssignClaimModel>()
        .WithDescription("Assign a claim to a user")
        .WithSummary("Assign a claim to a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsRevokeClaim, async ([FromServices] IClaimsService claimService,
            [FromBody] RevokeClaimModel inputModel) =>
        {
            return await claimService.RevokeClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim revoked successfully")
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<RevokeClaimModel>()
        .WithDescription("Revoke a claim from a user")
        .WithSummary("Revoke a claim from a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsDeleteClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] DeleteClaimModel inputModel) =>
        {
            return await claimsService.DeleteClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim deleted successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<DeleteClaimModel>()
        .WithDescription("Delete claim")
        .WithSummary("Delete claim");
    }
}