using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.RolesManager.DependencyInjection;
using MinimalApi.Identity.RolesManager.Extensions;
using MinimalApi.Identity.RolesManager.Models;

namespace MinimalApi.Identity.RolesManager.Endpoints;

public static class RolesEndpoints
{
    public static IEndpointRouteBuilder MapRolesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsRolesGroup)
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsRolesTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<List<RoleResponseModel>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
            .RequireAuthorization(nameof(Permissions.RuoloRead))
            .WithOpenApi(opt =>
            {
                opt.Summary = "Get all roles";
                opt.Description = "Get all roles";

                opt.Response(StatusCodes.Status200OK).Description = "List of roles";
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        apiGroup.MapPost(RolesExtensions.EndpointsCreateRole, EndpointsHandler.CreateRoleHandlerAsync)
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status409Conflict, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<CreateRoleModel>()
            .WithOpenApi(opt =>
            {
                opt.Summary = "Create role";
                opt.Description = "Create role";

                opt.Response(StatusCodes.Status200OK).Description = "Role created";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status409Conflict).Description = ConstantsConfiguration.Conflict;

                return opt;
            });

        apiGroup.MapPost(RolesExtensions.EndpointsAssignRole, EndpointsHandler.AssignRoleAsync)
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<AssignRoleModel>()
            .WithOpenApi(opt =>
            {
                opt.Summary = "Assign role";
                opt.Description = "Assign role to user";

                opt.Response(StatusCodes.Status200OK).Description = "Role assigned";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        apiGroup.MapDelete(RolesExtensions.EndpointsRevokeRole, EndpointsHandler.RevokeRoleAsync)
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<RevokeRoleModel>()
            .WithOpenApi(opt =>
            {
                opt.Summary = "Revoke role";
                opt.Description = "Revoke role from user";

                opt.Response(StatusCodes.Status200OK).Description = "Role revoked";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        apiGroup.MapDelete(RolesExtensions.EndpointsDeleteRole, EndpointsHandler.DeleteRoleAsync)
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<DeleteRoleModel>()
            .WithOpenApi(opt =>
            {
                opt.Summary = "Delete role";
                opt.Description = "Delete role";

                opt.Response(StatusCodes.Status200OK).Description = "Role deleted";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
                opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

                return opt;
            });

        return apiGroup;
    }
}