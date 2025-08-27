using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Utility.Generators;

namespace MinimalApi.Identity.API.Endpoints;

public class RolesEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsRolesGroup)
            .RequireAuthorization()
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsRolesTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, async ([FromServices] IRoleService roleService) =>
        {
            return await roleService.GetAllRolesAsync();
        })
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

        apiGroup.MapPost(EndpointsApi.EndpointsCreateRole, async ([FromServices] IRoleService roleService,
            [FromBody] CreateRoleModel inputModel) =>
        {
            return await roleService.CreateRoleAsync(inputModel);
        })
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

        apiGroup.MapPost(EndpointsApi.EndpointsAssignRole, async ([FromServices] IRoleService roleService,
            [FromBody] AssignRoleModel inputModel) =>
        {
            return await roleService.AssignRoleAsync(inputModel);
        })
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

        apiGroup.MapDelete(EndpointsApi.EndpointsRevokeRole, async ([FromServices] IRoleService roleService,
            [FromBody] RevokeRoleModel inputModel) =>
        {
            return await roleService.RevokeRoleAsync(inputModel);
        })
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

        apiGroup.MapDelete(EndpointsApi.EndpointsDeleteRole, async ([FromServices] IRoleService roleService,
            [FromBody] DeleteRoleModel inputModel) =>
        {
            return await roleService.DeleteRoleAsync(inputModel);
        })
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
    }
}