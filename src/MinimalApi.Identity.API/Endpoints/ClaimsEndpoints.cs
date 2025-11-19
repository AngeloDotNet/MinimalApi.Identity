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
            //.WithOpenApi(opt =>
            //{
            //    opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsClaimsTag }];
            //    return opt;
            //});
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, async ([FromServices] IClaimsService claimsService) =>
        {
            return await claimsService.GetAllClaimsAsync();
        })
        .Produces<Ok<List<ClaimResponseModel>>>(StatusCodes.Status200OK).WithDescription("Claims retrieved successfully")
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .RequireAuthorization(nameof(Permissions.ClaimRead))
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Get all claims";
        //    opt.Summary = "Get all claims";

        //    opt.Response(StatusCodes.Status200OK).Description = "Claims retrieved successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

        //    return opt;
        //})
        .WithDescription("Get all claims")
        .WithSummary("Get all claims");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsCreateClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] CreateClaimModel inputModel) =>
        {
            return await claimsService.CreateClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim created successfully")
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<CreateClaimModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Add claim";
        //    opt.Summary = "Add claim";

        //    opt.Response(StatusCodes.Status200OK).Description = "Claim added successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

        //    return opt;
        //})
        .WithDescription("Add claim")
        .WithSummary("Add claim");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAssignClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] AssignClaimModel inputModel) =>
        {
            await claimsService.AssignClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim assigned successfully")
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<AssignClaimModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Assign a claim to a user";
        //    opt.Summary = "Assign a claim to a user";

        //    opt.Response(StatusCodes.Status200OK).Description = "Claim assigned successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

        //    return opt;
        //})
        .WithDescription("Assign a claim to a user")
        .WithSummary("Assign a claim to a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsRevokeClaim, async ([FromServices] IClaimsService claimService,
            [FromBody] RevokeClaimModel inputModel) =>
        {
            await claimService.RevokeClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim revoked successfully")
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<RevokeClaimModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Revoke a claim from a user";
        //    opt.Summary = "Revoke a claim from a user";

        //    opt.Response(StatusCodes.Status200OK).Description = "Claim revoked successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

        //    return opt;
        //})
        .WithDescription("Revoke a claim from a user")
        .WithSummary("Revoke a claim from a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsDeleteClaim, async ([FromServices] IClaimsService claimsService,
            [FromBody] DeleteClaimModel inputModel) =>
        {
            await claimsService.DeleteClaimAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Claim deleted successfully")
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ClaimWrite))
        .WithValidation<DeleteClaimModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Delete claim";
        //    opt.Summary = "Delete claim";

        //    opt.Response(StatusCodes.Status200OK).Description = "Claim deleted successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

        //    return opt;
        //})
        .WithDescription("Delete claim")
        .WithSummary("Delete claim");
    }
}