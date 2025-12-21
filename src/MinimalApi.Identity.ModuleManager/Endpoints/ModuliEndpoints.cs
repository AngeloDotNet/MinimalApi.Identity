using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.ModuleManager.DependencyInjection;
using MinimalApi.Identity.ModuleManager.Extensions;
using MinimalApi.Identity.ModuleManager.Models;

namespace MinimalApi.Identity.API.Endpoints;

public static class ModuliEndpoints
{
    public static IEndpointRouteBuilder MapModuliEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsModulesGroup)
            .RequireAuthorization();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllModulesAsync)
            .Produces<Ok<List<ModuleResponseModel>>>(StatusCodes.Status200OK).WithDescription("Modules retrieved successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.ModuloRead))
            .WithDescription("Get all modules")
            .WithSummary("Get all modules");

        apiGroup.MapPost(ModuliExtensions.EndpointsCreateModule, EndpointsHandler.CreateModuleAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module created successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ModuloWrite))
            .WithValidation<CreateModuleModel>()
            .WithDescription("Create a new module")
            .WithSummary("Create a new module");

        apiGroup.MapPost(ModuliExtensions.EndpointsAssignModule, EndpointsHandler.AssignModuleAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module assigned successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ModuloWrite))
            .WithValidation<AssignModuleModel>()
            .WithDescription("Assign a module to a user")
            .WithSummary("Assign a module to a user");

        apiGroup.MapDelete(ModuliExtensions.EndpointsRevokeModule, EndpointsHandler.RevokeModuleAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module revoked successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ModuloWrite))
            .WithValidation<RevokeModuleModel>()
            .WithDescription("Revoke a module from a user")
            .WithSummary("Revoke a module from a user");

        apiGroup.MapDelete(ModuliExtensions.EndpointsDeleteModule, EndpointsHandler.DeleteModuleAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("Module deleted successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ModuloWrite))
            .WithValidation<DeleteModuleModel>()
            .WithDescription("Delete module")
            .WithSummary("Delete module");

        return apiGroup;
    }
}