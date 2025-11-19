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
            //.WithOpenApi(opt =>
            //{
            //    opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsModulesTag }];
            //    return opt;
            //});
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, async ([FromServices] IModuleService moduleService) =>
        {
            await moduleService.GetAllModulesAsync();
        })
        .Produces<Ok<List<ModuleResponseModel>>>(StatusCodes.Status200OK).WithDescription("Modules retrieved successfully")
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .RequireAuthorization(nameof(Permissions.ModuloRead))
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Get all modules";
        //    opt.Summary = "Get all modules";

        //    opt.Response(StatusCodes.Status200OK).Description = "Modules retrieved successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

        //    return opt;
        //})
        .WithDescription("Get all modules")
        .WithSummary("Get all modules");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsCreateModule, async ([FromServices] IModuleService moduleService,
            [FromBody] CreateModuleModel inputModel) =>
        {
            await moduleService.CreateModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module created successfully")
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<CreateModuleModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Create a new module";
        //    opt.Summary = "Create a new module";

        //    opt.Response(StatusCodes.Status200OK).Description = "Module created successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

        //    return opt;
        //})
        .WithDescription("Create a new module")
        .WithSummary("Create a new module");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAssignModule, async ([FromServices] IModuleService moduleService,
            [FromBody] AssignModuleModel inputModel) =>
        {
            await moduleService.AssignModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Module assigned successfully")
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<AssignModuleModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Assign a module to a user";
        //    opt.Summary = "Assign a module to a user";

        //    opt.Response(StatusCodes.Status200OK).Description = "Module assigned successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

        //    return opt;
        //})
        .WithDescription("Assign a module to a user")
        .WithSummary("Assign a module to a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsRevokeModule, async ([FromServices] IModuleService moduleService,
            [FromBody] RevokeModuleModel inputModel) =>
        {
            return await moduleService.RevokeModuleAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK)
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<RevokeModuleModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Revoke a module from a user";
        //    opt.Summary = "Revoke a module from a user";

        //    opt.Response(StatusCodes.Status200OK).Description = "Module revoked successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

        //    return opt;
        //})
        .WithDescription("Revoke a module from a user")
        .WithSummary("Revoke a module from a user");

        apiGroup.MapDelete(ConstantsConfiguration.EndpointsDeleteModule, async ([FromServices] IModuleService moduleService,
            [FromBody] DeleteModuleModel inputModel) =>
        {
            await moduleService.DeleteModuleAsync(inputModel);
        })
        .Produces<string>(StatusCodes.Status200OK)
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .RequireAuthorization(nameof(Permissions.ModuloWrite))
        .WithValidation<DeleteModuleModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Summary = "Delete module";
        //    opt.Description = "Delete module";

        //    opt.Response(StatusCodes.Status200OK).Description = "Module deleted successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

        //    return opt;
        //})
        .WithDescription("Delete module")
        .WithSummary("Delete module");
    }
}