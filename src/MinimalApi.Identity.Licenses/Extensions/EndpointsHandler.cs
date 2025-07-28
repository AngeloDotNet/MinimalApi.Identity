using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.Licenses.Models;
using MinimalApi.Identity.Licenses.Services.Interfaces;

namespace MinimalApi.Identity.Licenses.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> GetAllHandlerAsync([FromServices] ILicenseService licenseService, HttpContext httpContext)
    {
        var result = await licenseService.GetAllLicensesAsync(httpContext.RequestAborted);
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateHandlerAsync([FromBody] CreateLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext)
    {
        var result = await licenseService.CreateLicenseAsync(inputModel, httpContext.RequestAborted);
        return Results.Ok(result);
    }

    public static async Task<IResult> AssignHandlerAsync([FromBody] AssignLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext)
    {
        var result = await licenseService.AssignLicenseAsync(inputModel, httpContext.RequestAborted);
        return Results.Ok(result);
    }

    public static async Task<IResult> RevokeHandlerAsync([FromBody] RevokeLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext)
    {
        var result = await licenseService.RevokeLicenseAsync(inputModel, httpContext.RequestAborted);
        return Results.Ok(result);
    }

    public static async Task<IResult> DeleteHandlerAsync([FromBody] DeleteLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext)
    {
        var result = await licenseService.DeleteLicenseAsync(inputModel, httpContext.RequestAborted);
        return Results.Ok(result);
    }
}