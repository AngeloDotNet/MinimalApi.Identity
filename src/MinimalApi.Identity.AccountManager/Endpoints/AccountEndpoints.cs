using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.AccountManager.DependencyInjection;
using MinimalApi.Identity.AccountManager.Extensions;
using MinimalApi.Identity.AccountManager.Models;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Utility.Generators;

namespace MinimalApi.Identity.AccountManager.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsAccountGroup)
            .WithOpenApi(opt =>
            {
                opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsAccountTag }];
                return opt;
            });

        apiGroup.MapGet(EndpointGenerator.EndpointsConfirmEmail, EndpointsHandler.ConfirmEmailAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .WithOpenApi(opt =>
            {
                opt.Description = "Confirm email address";
                opt.Summary = "Confirm email address";

                opt.Response(StatusCodes.Status200OK).Description = "Email address confirmed successfully";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;

                return opt;
            });

        apiGroup.MapPost(AccountExtensions.EndpointChangeEmail, EndpointsHandler.ChangeEmailAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization()
            .WithValidation<ChangeEmailModel>()
            .WithOpenApi(opt =>
            {
                opt.Description = "Change email address";
                opt.Summary = "Change email address";

                opt.Response(StatusCodes.Status200OK).Description = "Email address changed successfully";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
                opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

                return opt;
            });

        apiGroup.MapGet(EndpointGenerator.EndpointsConfirmEmailChange, EndpointsHandler.ConfirmEmailChangeAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK)
            .ProducesDefaultProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .WithOpenApi(opt =>
            {
                opt.Description = "Confirm email address change";
                opt.Summary = "Confirm email address change";

                opt.Response(StatusCodes.Status200OK).Description = "Email address change confirmed successfully";
                opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;

                return opt;
            });

        return apiGroup;
    }
}