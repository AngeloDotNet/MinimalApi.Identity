using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.RolesManager.Models;
using MinimalApi.Identity.RolesManager.Services;

namespace MinimalApi.Identity.RolesManager.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> GetAllHandlerAsync([FromServices] IRoleService roleService, HttpContext httpContext)
        => Results.Ok(await roleService.GetAllRolesAsync(httpContext.RequestAborted));

    public static async Task<IResult> CreateRoleHandlerAsync([FromServices] IRoleService roleService, [FromBody] CreateRoleModel inputModel,
        HttpContext httpContext) => Results.Ok(await roleService.CreateRoleAsync(inputModel, httpContext.RequestAborted));

    public static async Task<IResult> AssignRoleAsync([FromServices] IRoleService roleService, [FromBody] AssignRoleModel inputModel,
        HttpContext httpContext) => Results.Ok(await roleService.AssignRoleAsync(inputModel, httpContext.RequestAborted));

    public static async Task<IResult> RevokeRoleAsync([FromServices] IRoleService roleService, [FromBody] RevokeRoleModel inputModel,
        HttpContext httpContext) => Results.Ok(await roleService.RevokeRoleAsync(inputModel, httpContext.RequestAborted));

    public static async Task<IResult> DeleteRoleAsync([FromServices] IRoleService roleService, [FromBody] DeleteRoleModel inputModel,
        HttpContext httpContext) => Results.Ok(await roleService.DeleteRoleAsync(inputModel, httpContext.RequestAborted));
}