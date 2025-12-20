using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.ModuleManager.Models;
using MinimalApi.Identity.ModuleManager.Services;

namespace MinimalApi.Identity.ModuleManager.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> GetAllModulesAsync([FromServices] IModuleService moduleService)
    {
        var result = await moduleService.GetAllModulesAsync();
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateModuleAsync([FromServices] IModuleService moduleService, [FromBody] CreateModuleModel inputModel)
    {
        var result = await moduleService.CreateModuleAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> AssignModuleAsync([FromServices] IModuleService moduleService, [FromBody] AssignModuleModel inputModel)
    {
        var result = await moduleService.AssignModuleAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> RevokeModuleAsync([FromServices] IModuleService moduleService, [FromBody] RevokeModuleModel inputModel)
    {
        var result = await moduleService.RevokeModuleAsync(inputModel);
        return Results.Ok(result);
    }

    public static async Task<IResult> DeleteModuleAsync([FromServices] IModuleService moduleService, [FromBody] DeleteModuleModel inputModel)
    {
        var result = await moduleService.DeleteModuleAsync(inputModel);
        return Results.Ok(result);
    }
}