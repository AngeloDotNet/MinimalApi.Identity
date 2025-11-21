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

public class ModuliEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsModulesGroup)
            .RequireAuthorization()
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, async ([FromServices] IModuleService moduleService) =>
        {
            return await moduleService.GetAllModulesAsync();
        })
        .Produces<Ok<List<ModuleResponseModel>>>(StatusCodes.Status200OK).WithDescription("Modules retrieved successfully")
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .RequireAuthorization(nameof(Permissions.ModuloRead))
        .WithDescription("Get all modules")
        .WithSummary("Get all modules");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsCreateModule, async ([FromServices] IModuleService moduleService,
            [FromBody] CreateModuleModel inputModel) =>
        {
            return await moduleService.CreateModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module created successfully")
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<CreateModuleModel>()
        .WithDescription("Create a new module")
        .WithSummary("Create a new module");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAssignModule, async ([FromServices] IModuleService moduleService,
            [FromBody] AssignModuleModel inputModel) =>
        {
            return await moduleService.AssignModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module assigned successfully")
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<AssignModuleModel>()
        .WithDescription("Assign a module to a user")
        .WithSummary("Assign a module to a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsRevokeModule, async ([FromServices] IModuleService moduleService,
            [FromBody] RevokeModuleModel inputModel) =>
        {
            return await moduleService.RevokeModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module revoked successfully")
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<RevokeModuleModel>()
        .WithDescription("Revoke a module from a user")
        .WithSummary("Revoke a module from a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsDeleteModule, async ([FromServices] IModuleService moduleService,
            [FromBody] DeleteModuleModel inputModel) =>
        {
            return await moduleService.DeleteModuleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK).WithDescription("Module deleted successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<DeleteModuleModel>()
        .WithDescription("Delete module")
        .WithSummary("Delete module");
    }
}