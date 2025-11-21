using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<List<RoleResponseModel>>(StatusCodes.Status200OK).WithDescription("Get all roles")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.RuoloRead))
            .WithDescription("Get all roles")
            .WithSummary("Get all roles");

        apiGroup.MapPost(RolesExtensions.EndpointsCreateRole, EndpointsHandler.CreateRoleHandlerAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("Role created")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict).WithDescription(ConstantsConfiguration.Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<CreateRoleModel>()
            .WithDescription("Create role")
            .WithSummary("Create role");

        apiGroup.MapPost(RolesExtensions.EndpointsAssignRole, EndpointsHandler.AssignRoleAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("Assign role to user")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<AssignRoleModel>()
            .WithDescription("Assign role to user")
            .WithSummary("Assign role");

        apiGroup.MapDelete(RolesExtensions.EndpointsRevokeRole, EndpointsHandler.RevokeRoleAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("Revoke role from user")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<RevokeRoleModel>()
            .WithDescription("Revoke role from user")
            .WithSummary("Revoke role");

        apiGroup.MapDelete(RolesExtensions.EndpointsDeleteRole, EndpointsHandler.DeleteRoleAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("Delete role")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .RequireAuthorization(nameof(Permissions.RuoloWrite))
            .WithValidation<DeleteRoleModel>()
            .WithDescription("Delete role")
            .WithSummary("Delete role");

        return apiGroup;
    }
}