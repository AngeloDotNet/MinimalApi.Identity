using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.LicenseManager.Models;
using MinimalApi.Identity.LicenseManager.Services;

namespace MinimalApi.Identity.LicenseManager.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> GetAllHandlerAsync([FromServices] ILicenseService licenseService, HttpContext httpContext)
        => Results.Ok(await licenseService.GetAllLicensesAsync(httpContext.RequestAborted));

    public static async Task<IResult> CreateHandlerAsync([FromBody] CreateLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext) => Results.Ok(await licenseService.CreateLicenseAsync(inputModel, httpContext.RequestAborted));

    public static async Task<IResult> AssignHandlerAsync([FromBody] AssignLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext) => Results.Ok(await licenseService.AssignLicenseAsync(inputModel, httpContext.RequestAborted));

    public static async Task<IResult> RevokeHandlerAsync([FromBody] RevokeLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext) => Results.Ok(await licenseService.RevokeLicenseAsync(inputModel, httpContext.RequestAborted));

    public static async Task<IResult> DeleteHandlerAsync([FromBody] DeleteLicenseModel inputModel, [FromServices] ILicenseService licenseService,
        HttpContext httpContext) => Results.Ok(await licenseService.DeleteLicenseAsync(inputModel, httpContext.RequestAborted));
}