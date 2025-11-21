using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Utility.Generators;

namespace MinimalApi.Identity.API.Endpoints;

public class AuthEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsAuthGroup)
            .RequireAuthorization()
            .WithOpenApi();

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthRegister, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] RegisterModel inputModel) =>
        {
            return await authService.RegisterAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("User registered successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .WithValidation<RegisterModel>()
        .WithName("Register")
        .WithDescription("Register new user")
        .WithSummary("Register new user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthLogin, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] LoginModel inputModel) =>
        {
            return await authService.LoginAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("User logged in successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .WithValidation<LoginModel>()
        .WithDescription("Login user")
        .WithSummary("Login user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthRefreshToken, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] RefreshTokenModel inputModel) =>
        {
            await authService.RefreshTokenAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("Token refreshed successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .WithValidation<RefreshTokenModel>()
        .WithDescription("Refresh token user")
        .WithSummary("Refresh token user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsImpersonateUser, async ([FromServices] IAuthService authService,
            [FromBody] ImpersonateUserModel inputModel) =>
        {
            return await authService.ImpersonateAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("User impersonated successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .WithValidation<ImpersonateUserModel>()
        .WithDescription("Impersonate user")
        .WithSummary("Impersonate user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthLogout, [AllowAnonymous] async ([FromServices] IAuthService authService) =>
        {
            return await authService.LogoutAsync().ConfigureAwait(false);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("User logged out successfully")
        .WithDescription("Logout user")
        .WithSummary("Logout user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsForgotPassword, async ([FromServices] IAuthService authService,
            [FromBody] ForgotPasswordModel inputModel) =>
        {
            return await authService.ForgotPasswordAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Password reset link sent successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .WithValidation<ForgotPasswordModel>()
        .WithDescription("Forgot password")
        .WithSummary("Forgot password");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsResetPassword, async ([FromServices] IAuthService authService,
            [FromBody] ResetPasswordModel inputModel, [FromRoute] string code) =>
        {
            return await authService.ResetPasswordAsync(inputModel, code);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Your password has been reset.")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        .WithValidation<ResetPasswordModel>()
        .WithDescription("Reset password")
        .WithSummary("Reset password");
    }
}