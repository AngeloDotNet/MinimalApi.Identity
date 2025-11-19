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
        //var apiGroup = endpoints
        //    .MapGroup(EndpointGenerator.EndpointsAuthGroup)
        //    .RequireAuthorization()
        //    .WithOpenApi(opt =>
        //    {
        //        opt.Tags = [new OpenApiTag { Name = EndpointGenerator.EndpointsAuthTag }];
        //        return opt;
        //    });

        var apiGroup = endpoints
            .MapGroup(EndpointGenerator.EndpointsAuthGroup)
            .RequireAuthorization()
            .WithOpenApi();

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthRegister, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] RegisterModel inputModel) =>
        {
            await authService.RegisterAsync(inputModel);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("User registered successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<RegisterModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Register new user";
        //    opt.Summary = "Register new user";

        //    opt.Response(StatusCodes.Status200OK).Description = "User registered successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;

        //    return opt;
        //})
        //.WithOpenApi()
        .WithName("Register")
        .WithDescription("Register new user")
        .WithSummary("Register new user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthLogin, [AllowAnonymous] async ([FromServices] IAuthService authService,
            [FromBody] LoginModel inputModel) =>
        {
            await authService.LoginAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("User logged in successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<LoginModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Login user";
        //    opt.Summary = "Login user";

        //    opt.Response(StatusCodes.Status200OK).Description = "User logged in successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    return opt;
        //});
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
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<RefreshTokenModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Refresh token user";
        //    opt.Summary = "Refresh token user";

        //    opt.Response(StatusCodes.Status200OK).Description = "Token refreshed successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    return opt;
        //});
        .WithDescription("Refresh token user")
        .WithSummary("Refresh token user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsImpersonateUser, async ([FromServices] IAuthService authService,
            [FromBody] ImpersonateUserModel inputModel) =>
        {
            await authService.ImpersonateAsync(inputModel);
        })
        .Produces<Ok<AuthResponseModel>>(StatusCodes.Status200OK).WithDescription("User impersonated successfully")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<ImpersonateUserModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Impersonate user";
        //    opt.Summary = "Impersonate user";

        //    opt.Response(StatusCodes.Status200OK).Description = "User impersonated successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    return opt;
        //});
        .WithDescription("Impersonate user")
        .WithSummary("Impersonate user");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsAuthLogout, [AllowAnonymous] async ([FromServices] IAuthService authService) =>
        {
            await authService.LogoutAsync();
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("User logged out successfully")
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Logout user";
        //    opt.Summary = "Logout user";

        //    opt.Response(StatusCodes.Status200OK).Description = "User logged out successfully";

        //    return opt;
        //});
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
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<ForgotPasswordModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Forgot password";
        //    opt.Summary = "Forgot password";

        //    opt.Response(StatusCodes.Status200OK).Description = "Password reset link sent successfully";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    return opt;
        //});
        .WithDescription("Forgot password")
        .WithSummary("Forgot password");

        apiGroup.MapPost(ConstantsConfiguration.EndpointsResetPassword, async ([FromServices] IAuthService authService,
            [FromBody] ResetPasswordModel inputModel, [FromRoute] string code) =>
        {
            await authService.ResetPasswordAsync(inputModel, code);
        })
        .Produces<Ok<string>>(StatusCodes.Status200OK).WithDescription("Your password has been reset.")
        .ProducesProblem(StatusCodes.Status400BadRequest).WithDescription(ConstantsConfiguration.BadRequest)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity).WithDescription("Validation error")
        //.ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status422UnprocessableEntity)
        .WithValidation<ResetPasswordModel>()
        //.WithOpenApi(opt =>
        //{
        //    opt.Description = "Reset password";
        //    opt.Summary = "Reset password";

        //    opt.Response(StatusCodes.Status200OK).Description = "Your password has been reset.";
        //    opt.Response(StatusCodes.Status400BadRequest).Description = ConstantsConfiguration.BadRequest;
        //    return opt;
        //});
        .WithDescription("Reset password")
        .WithSummary("Reset password");
    }
}