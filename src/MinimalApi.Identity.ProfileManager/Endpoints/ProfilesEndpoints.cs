using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.ProfileManager.DependencyInjection;
using MinimalApi.Identity.ProfileManager.Extensions;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.API.Endpoints;

public class ProfilesEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsProfilesGroup)
            .WithTags(EndpointGenerator.EndpointsProfilesTag);

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<List<UserProfileModel>>(StatusCodes.Status200OK).WithDescription("List of users profiles")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.ProfiloRead))
            .WithDescription("Get all profiles")
            .WithSummary("Get all profiles");

        apiGroup.MapGet(ProfileExtensions.EndpointsGetProfile, EndpointsHandler.GetProfileHandlerAsync)
            .Produces<UserProfileModel>(StatusCodes.Status200OK).WithDescription("User profile retrieved successfully")
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.ProfiloRead))
            .WithDescription("Get user profile")
            .WithSummary("Get user profile");

        apiGroup.MapPost(ProfileExtensions.EndpointsChangeEnableProfile, EndpointsHandler.ChangeUserStatusAsync)
            .Produces<UserProfileModel>(StatusCodes.Status200OK).WithDescription("User profile enablement changed successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.ProfiloRead))
            .WithDescription("Edit user profile enablement")
            .WithSummary("Edit user profile enablement");

        apiGroup.MapPut(ProfileExtensions.EndpointsEditProfile, EndpointsHandler.EditProfileAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("User profile updated successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound).WithDescription(ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .RequireAuthorization(nameof(Permissions.ProfiloWrite))
            .WithValidation<EditUserProfileModel>()
            .WithDescription("Update user profile")
            .WithSummary("Update user profile");
    }
}