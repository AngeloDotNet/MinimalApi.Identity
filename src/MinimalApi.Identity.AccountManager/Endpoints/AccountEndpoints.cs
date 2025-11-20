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
            //.WithOpenApi(opt =>
            //{
            //    opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsAccountTag }];
            //    return opt;
            //})
            .WithOpenApi();

        apiGroup.MapGet(EndpointGenerator.EndpointsConfirmEmail, EndpointsHandler.ConfirmEmailAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Email address confirmed successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            //.ProducesDefaultProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            //.WithOpenApi(opt =>
            //{
            //    opt.Description = "Confirm email address";
            //    opt.Summary = "Confirm email address";

            //    opt.Response(StatusCodes.Status200OK).Description = "Email address confirmed successfully";
            //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;

            //    return opt;
            //})
            .WithDescription("Confirm email address")
            .WithSummary("Confirm email address");

        apiGroup.MapPost(AccountExtensions.EndpointChangeEmail, EndpointsHandler.ChangeEmailAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Email address changed successfully")
            //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized).WithDescription(ConstantsConfiguration.Unauthorized)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
            //.RequireAuthorization()
            .WithValidation<ChangeEmailModel>()
            //.WithOpenApi(opt =>
            //{
            //    opt.Description = "Change email address";
            //    opt.Summary = "Change email address";

            //    opt.Response(StatusCodes.Status200OK).Description = "Email address changed successfully";
            //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
            //    opt.Response(StatusCodes.Status401Unauthorized).Description = ConstantsConfiguration.Unauthorized;

            //    return opt;
            //})
            .WithDescription("Change email address")
            .WithSummary("Change email address");

        apiGroup.MapGet(EndpointGenerator.EndpointsConfirmEmailChange, EndpointsHandler.ConfirmEmailChangeAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Email address change confirmed successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            //.ProducesDefaultProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            //.WithOpenApi(opt =>
            //{
            //    opt.Description = "Confirm email address change";
            //    opt.Summary = "Confirm email address change";

            //    opt.Response(StatusCodes.Status200OK).Description = "Email address change confirmed successfully";
            //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;

            //    return opt;
            //})
            .WithDescription("Confirm email address change")
            .WithSummary("Confirm email address change");

        return apiGroup;
    }
}