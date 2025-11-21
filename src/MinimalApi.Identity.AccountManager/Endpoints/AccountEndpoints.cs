using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
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
            .RequireAuthorization()
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsConfirmEmail, EndpointsHandler.ConfirmEmailAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Email address confirmed successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .AllowAnonymous()
            .WithDescription("Confirm email address")
            .WithSummary("Confirm email address");

        apiGroup.MapPost(AccountExtensions.EndpointChangeEmail, EndpointsHandler.ChangeEmailAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Email address changed successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            .WithValidation<ChangeEmailModel>()
            .WithDescription("Change email address")
            .WithSummary("Change email address");

        apiGroup.MapGet(EndpointGenerator.EndpointsConfirmEmailChange, EndpointsHandler.ConfirmEmailChangeAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Email address change confirmed successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .AllowAnonymous()
            .WithDescription("Confirm email address change")
            .WithSummary("Confirm email address change");

        return apiGroup;
    }
}