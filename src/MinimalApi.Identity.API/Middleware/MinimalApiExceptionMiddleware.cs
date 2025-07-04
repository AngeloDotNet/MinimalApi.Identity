﻿using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Exceptions.Conflict;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Exceptions.Users;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Middleware;

public class MinimalApiExceptionMiddleware(RequestDelegate next, IOptions<ValidationOptions> options)
{
    private readonly ValidationOptions validationOptions = options.Value;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, validationOptions);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ValidationOptions validationOptions)
    {
        var statusCode = GetStatusCodeFromException(exception);
        var message = GetMessageFromException(exception);
        var problemDetails = CreateProblemDetails(context, statusCode, message);

        if (exception is ValidationModelException validationException)
        {
            problemDetails.Extensions["errors"] = validationOptions.ErrorResponseFormat == ErrorResponseFormat.List
                ? validationException.Errors.SelectMany(e
                    => e.Value.Select(m => new { Name = e.Key, Message = m })).ToArray() : validationException.Errors;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }

    public static ProblemDetails CreateProblemDetails(HttpContext context, HttpStatusCode statusCode, string detail)
    {
        var user = context.Features.Get<IHttpAuthenticationFeature>()?.User;
        var type = $"https://httpstatuses.io/{(int)statusCode}";

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Type = type,
            Title = MessageApi.ProblemDetailsMessageTitle,
            Instance = $"{context.Request.Method} {context.Request.Path}",
            Detail = detail,
            Extensions = {
                    ["traceId"] = context.Features.Get<IHttpActivityFeature>()?.Activity.Id,
                    ["requestId"] = context.TraceIdentifier,
                    ["dateTime"] = DateTime.UtcNow
                }
        };

        if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            var stackTrace = context.Features.Get<IExceptionHandlerFeature>()?.Error?.StackTrace;

            if (!string.IsNullOrEmpty(stackTrace))
            {
                problemDetails.Extensions["stackTrace"] = stackTrace;
            }
        }

        if (user?.Identity?.IsAuthenticated == true)
        {
            var claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);

            if (claims.TryGetValue(ClaimTypes.NameIdentifier, out var userId))
            {
                problemDetails.Extensions["userId"] = userId;
            }

            if (claims.TryGetValue(ClaimTypes.Name, out var userName))
            {
                problemDetails.Extensions["userName"] = userName;
            }
        }

        return problemDetails;
    }

    private static HttpStatusCode GetStatusCodeFromException(Exception exception)
        => exception switch
        {
            ArgumentOutOfRangeException or ArgumentNullException => HttpStatusCode.BadRequest,

            BadRequestException or
            BadRequestClaimException or
            BadRequestModuleException or
            BadRequestPolicyException or
            BadRequestProfileException or
            BadRequestRoleException or
            BadRequestUserException => HttpStatusCode.BadRequest,

            ConflictException or
            ConflictClaimException or
            ConflictPolicyException or
            ConflictModuleException or
            ConflictRoleException => HttpStatusCode.Conflict,

            NotFoundException or
            NotFoundActivePoliciesException or
            NotFoundClaimException or
            NotFoundModuleException or
            NotFoundPolicyException or
            NotFoundProfileException or
            NotFoundRoleException or
            NotFoundUserException => HttpStatusCode.NotFound,

            UserIsLockedException or
            UserTokenIsInvalidException or
            UserUnknownException or
            UserWithoutPermissionsException => HttpStatusCode.Unauthorized,

            ValidationModelException => HttpStatusCode.UnprocessableEntity,
            _ => HttpStatusCode.InternalServerError
        };

    private static string GetMessageFromException(Exception exception)
        => exception switch
        {
            ArgumentOutOfRangeException argumentOutOfRangeException => argumentOutOfRangeException.Message,
            ArgumentNullException argumentNullException => argumentNullException.Message,

            BadRequestException badRequestException => badRequestException.Message,
            BadRequestClaimException badRequestClaimException => badRequestClaimException.Message,
            BadRequestModuleException badRequestModuleException => badRequestModuleException.Message,
            BadRequestPolicyException badRequestPolicyException => badRequestPolicyException.Message,
            BadRequestProfileException badRequestProfileException => badRequestProfileException.Message,
            BadRequestRoleException badRequestRoleException => badRequestRoleException.Message,
            BadRequestUserException badRequestUserException => badRequestUserException.Message,

            ConflictException conflictException => conflictException.Message,
            ConflictClaimException conflictClaimException => conflictClaimException.Message,
            ConflictModuleException conflictModuleException => conflictModuleException.Message,
            ConflictPolicyException conflictPolicyException => conflictPolicyException.Message,
            ConflictRoleException conflictRoleException => conflictRoleException.Message,

            NotFoundException notFoundLicenseException => notFoundLicenseException.Message,
            NotFoundActivePoliciesException notFoundActivePoliciesException => notFoundActivePoliciesException.Message,
            NotFoundClaimException notFoundClaimException => notFoundClaimException.Message,
            NotFoundModuleException notFoundModuleException => notFoundModuleException.Message,
            NotFoundPolicyException notFoundPolicyException => notFoundPolicyException.Message,
            NotFoundProfileException notFoundProfileException => notFoundProfileException.Message,
            NotFoundRoleException notFoundRoleException => notFoundRoleException.Message,
            NotFoundUserException notFoundUserException => notFoundUserException.Message,

            UserIsLockedException => MessageApi.UserLockedOut,
            UserTokenIsInvalidException userTokenIsInvalidException => userTokenIsInvalidException.Message,
            UserUnknownException => MessageApi.UserNotAuthenticated,
            UserWithoutPermissionsException => MessageApi.UserNotHavePermission,

            ValidationModelException validationModelException => validationModelException.Message,
            _ => MessageApi.UnexpectedError
        };
}