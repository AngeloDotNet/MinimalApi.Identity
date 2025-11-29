using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.LicenseManager.DependencyInjection;
using MinimalApi.Identity.LicenseManager.Extensions;
using MinimalApi.Identity.LicenseManager.Models;

namespace MinimalApi.Identity.LicenseManager.Endpoints;

public static class LicenseEndpoints
{
    public static IEndpointRouteBuilder MapLicenseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsLicenseGroup);

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<Ok<List<LicenseResponseModel>>>(StatusCodes.Status200OK).WithDescription("Licenses retrieved successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.LicenzaRead))
            .WithDescription("Get all licenses")
            .WithSummary("Get all licenses");

        apiGroup.MapPost(LicenseExtensions.EndpointsCreateLicense, EndpointsHandler.CreateHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("License created successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<CreateLicenseModel>()
            .WithDescription("Create a new license")
            .WithSummary("Create a new license");

        apiGroup.MapPost(LicenseExtensions.EndpointsAssignLicense, EndpointsHandler.AssignHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("License assigned successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<AssignLicenseModel>()
            .WithDescription("Assign a license to a user")
            .WithSummary("Assign a license to a user");

        apiGroup.MapDelete(LicenseExtensions.EndpointsRevokeLicense, EndpointsHandler.RevokeHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("License revoked successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<RevokeLicenseModel>()
            .WithDescription("Revoke a license from a user")
            .WithSummary("Revoke a license from a user");

        apiGroup.MapDelete(LicenseExtensions.EndpointsDeleteLicense, EndpointsHandler.DeleteHandlerAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("License deleted successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<DeleteLicenseModel>()
            .WithDescription("Delete license")
            .WithSummary("Delete license");

        return apiGroup;
    }
}