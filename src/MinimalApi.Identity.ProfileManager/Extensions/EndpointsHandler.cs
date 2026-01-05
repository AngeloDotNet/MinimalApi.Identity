using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.ProfileManager.Models;
using MinimalApi.Identity.ProfileManager.Services;

namespace MinimalApi.Identity.ProfileManager.Extensions;

public static class EndpointsHandler
{
    public static async Task<IResult> GetAllHandlerAsync([FromServices] IProfileService profileService, HttpContext httpContext)
        => Results.Ok(await profileService.GetAllProfilesAsync(httpContext.RequestAborted));

    public static async Task<IResult> GetProfileHandlerAsync([FromServices] IProfileService profileService, [FromRoute] int userId,
        HttpContext httpContext) => Results.Ok(await profileService.GetProfileAsync(userId, httpContext.RequestAborted));

    public static async Task<IResult> EditProfileAsync([FromServices] IProfileService profileService, [FromBody] EditUserProfileModel inputModel,
        HttpContext httpContext) => Results.Ok(await profileService.EditProfileAsync(inputModel, httpContext.RequestAborted));

    public static async Task<IResult> ChangeUserStatusAsync([FromServices] IProfileService profileService, [FromBody] ChangeEnableProfileModel inputModel,
        HttpContext httpContext) => Results.Ok(await profileService.ChangeEnablementStatusUserProfileAsync(inputModel, httpContext.RequestAborted));
}