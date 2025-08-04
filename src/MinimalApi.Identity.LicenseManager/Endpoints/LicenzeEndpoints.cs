using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.LicenseManager.DependencyInjection;
using MinimalApi.Identity.LicenseManager.Extensions;
using MinimalApi.Identity.LicenseManager.Models;

namespace MinimalApi.Identity.LicenseManager.Endpoints;

public static class LicenzeEndpoints
{
    public static IEndpointRouteBuilder MapLicenseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsLicenseGroup)
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsLicenseTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<Ok<List<LicenseResponseModel>>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
            .RequireAuthorization(nameof(Permissions.LicenzaRead))
            .WithOpenApi(opt =>
            {
                opt.Description = "Get all licenses";
                opt.Summary = "Get all licenses";

                opt.Response(StatusCodes.Status200OK).Description = "Licenses retrieved successfully";
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        apiGroup.MapPost(LicenseExtensions.EndpointsCreateLicense, EndpointsHandler.CreateHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<CreateLicenseModel>()
            .WithOpenApi(opt =>
            {
                opt.Description = "Create a new license";
                opt.Summary = "Create a new license";

                opt.Response(StatusCodes.Status200OK).Description = "License created successfully";
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

                return opt;
            });

        apiGroup.MapPost(LicenseExtensions.EndpointsAssignLicense, EndpointsHandler.AssignHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<AssignLicenseModel>()
            .WithOpenApi(opt =>
            {
                opt.Description = "Assign a license to a user";
                opt.Summary = "Assign a license to a user";

                opt.Response(StatusCodes.Status200OK).Description = "License assigned successfully";
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        apiGroup.MapDelete(LicenseExtensions.EndpointsRevokeLicense, EndpointsHandler.RevokeHandlerAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<RevokeLicenseModel>()
            .WithOpenApi(opt =>
            {
                opt.Description = "Revoke a license from a user";
                opt.Summary = "Revoke a license from a user";

                opt.Response(StatusCodes.Status200OK).Description = "License revoked successfully";
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        apiGroup.MapDelete(LicenseExtensions.EndpointsDeleteLicense, EndpointsHandler.DeleteHandlerAsync)
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.LicenzaWrite))
            .WithValidation<DeleteLicenseModel>()
            .WithOpenApi(opt =>
            {
                opt.Summary = "Delete license";
                opt.Description = "Delete license";

                opt.Response(StatusCodes.Status200OK).Description = "License deleted successfully";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        return apiGroup;
    }
}