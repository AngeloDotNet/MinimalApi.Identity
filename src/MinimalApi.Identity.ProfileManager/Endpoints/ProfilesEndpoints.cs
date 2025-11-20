using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.ProfileManager.DependencyInjection;
using MinimalApi.Identity.ProfileManager.Extensions;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.API.Endpoints;

public static class ProfilesEndpoints
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsProfilesGroup)
            //.WithOpenApi(opt =>
            //{
            //    opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsProfilesTag }];
            //    return opt;
            //});
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsStringEmpty, EndpointsHandler.GetAllHandlerAsync)
            .Produces<List<UserProfileModel>>(StatusCodes.Status200OK).WithDescription("List of users profiles")
            //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized, ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound, ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.ProfiloRead))
            //.WithOpenApi(opt =>
            //{
            //    opt.Summary = "Get all profiles";
            //    opt.Description = "Get all profiles";

            //    opt.Response(StatusCodes.Status200OK).Description = "List of users profiles";
            //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
            //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

            //    return opt;
            //});
            .WithDescription("Get all profiles")
            .WithSummary("Get all profiles");

        apiGroup.MapGet(ProfileExtensions.EndpointsGetProfile, EndpointsHandler.GetProfileHandlerAsync)
        .Produces<UserProfileModel>(StatusCodes.Status200OK).WithDescription("User profile retrieved successfully")
        //.ProducesDefaultProblem(StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized, ConstantsConfiguration.Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound, ConstantsConfiguration.NotFound)
        .RequireAuthorization(nameof(Permissions.ProfiloRead))
        //.WithOpenApi(opt =>
        //{
        //    opt.Summary = "Get user profile";
        //    opt.Description = "Get user profile by username";

        //    opt.Response(StatusCodes.Status200OK).Description = "User profile retrieved successfully";
        //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
        //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;
        //    return opt;
        //});
        .WithDescription("Get user profile")
        .WithSummary("Get user profile");

        apiGroup.MapPost(ProfileExtensions.EndpointsChangeEnableProfile, EndpointsHandler.ChangeUserStatusAsync)
            .Produces<UserProfileModel>(StatusCodes.Status200OK).WithDescription("User profile enablement changed successfully")
            //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized, ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound, ConstantsConfiguration.NotFound)
            .RequireAuthorization(nameof(Permissions.ProfiloRead))
            //.WithOpenApi(opt =>
            //{
            //    opt.Summary = "Edit user profile enablement";
            //    opt.Description = "Edit user profile enablement";

            //    opt.Response(StatusCodes.Status200OK).Description = "User profile retrieved successfully";
            //    opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
            //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;
            //    return opt;
            //});
            .WithDescription("Edit user profile enablement")
            .WithSummary("Edit user profile enablement");

        apiGroup.MapPut(ProfileExtensions.EndpointsEditProfile, EndpointsHandler.EditProfileAsync)
            .Produces<string>(StatusCodes.Status200OK).WithDescription("User profile updated successfully")
            //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized, ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound, ConstantsConfiguration.NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .RequireAuthorization(nameof(Permissions.ProfiloWrite))
            .WithValidation<EditUserProfileModel>()
            //.WithOpenApi(opt =>
            //{
            //    opt.Summary = "Update user profile";
            //    opt.Description = "Update user profile by username";

            //    opt.Response(StatusCodes.Status200OK).Description = "User profile updated successfully";
            //    opt.Response(StatusCodes.Status400BadRequest).Description = "Bad request";
            //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;
            //    opt.Response(StatusCodes.Status404NotFound).Description = ConstantsConfiguration.NotFound;

            //    return opt;
            //});
            .WithDescription("Update user profile")
            .WithSummary("Update user profile");

        return apiGroup;
    }
}