using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
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
            .WithTags(EndpointGenerator.EndpointsAuthTag)
            .RequireAuthorization();

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthRegister, EndpointsHandler.RegisterAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("User registered successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .WithValidation<RegisterModel>()
            .WithName("Register")
            .WithDescription("Register new user")
            .WithSummary("Register new user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthLogin, EndpointsHandler.LoginAsync)
            .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("User logged in successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .WithValidation<LoginModel>()
            .WithName("Login")
            .WithDescription("Login user")
            .WithSummary("Login user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthRefreshToken, EndpointsHandler.RefreshTokenAsync)
            .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("Token refreshed successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .WithValidation<RefreshTokenModel>()
            .WithName("RefreshToken")
            .WithDescription("Refresh token user")
            .WithSummary("Refresh token user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsImpersonateUser, EndpointsHandler.ImpersonateUseAsync)
            .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("User impersonated successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .WithValidation<ImpersonateUserModel>()
            .WithName("ImpersonateUser")
            .WithDescription("Impersonate user")
            .WithSummary("Impersonate user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthLogout, EndpointsHandler.LogoutAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("User logged out successfully")
            .WithName("Logout")
            .WithDescription("Logout user")
            .WithSummary("Logout user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsForgotPassword, EndpointsHandler.ForgotPasswordAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Password reset link sent successfully")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .WithValidation<ForgotPasswordModel>()
            .WithName("ForgotPassword")
            .WithDescription("Forgot password")
            .WithSummary("Forgot password");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsResetPassword, EndpointsHandler.ResetPasswordAsync)
            .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Your password has been reset.")
            .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription(ConstantsConfiguration.ValidationErrors)
            .WithValidation<ResetPasswordModel>()
            .WithName("ResetPassword")
            .WithDescription("Reset password")
            .WithSummary("Reset password");
    }
}