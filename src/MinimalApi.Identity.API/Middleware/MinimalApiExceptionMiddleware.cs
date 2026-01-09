using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;

namespace MinimalApi.Identity.API.Middleware;

public class MinimalApiExceptionMiddleware(RequestDelegate next, ErrorResponseFormat errorResponseFormat)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, errorResponseFormat).ConfigureAwait(false);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ErrorResponseFormat errorResponseFormat)
    {
        var statusCode = GetStatusCodeFromException(exception);
        var message = GetMessageFromException(exception);
        var problemDetails = CreateProblemDetails(context, statusCode, message);

        if (exception is ValidationModelException validationException)
        {
            problemDetails.Extensions["errors"] = errorResponseFormat switch
            {
                ErrorResponseFormat.List => CreateErrorList(validationException),
                ErrorResponseFormat.Default => validationException.Errors,
                _ => CreateErrorList(validationException)
            };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json).ConfigureAwait(false);
    }

    public static ProblemDetails CreateProblemDetails(HttpContext context, HttpStatusCode statusCode, string detail)
    {
        var user = context.Features.Get<IHttpAuthenticationFeature>()?.User;
        var type = $"https://httpstatuses.io/{(int)statusCode}";

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Type = type,
            Title = MessagesExceptions.ProblemDetailsMessageTitle,
            Instance = $"{context.Request.Method} {context.Request.Path}",
            Detail = detail
        };

        var extensions = problemDetails.Extensions;
        extensions["traceId"] = context.Features.Get<IHttpActivityFeature>()?.Activity?.Id;
        extensions["requestId"] = context.TraceIdentifier;
        extensions["dateTime"] = DateTime.UtcNow;

        AddStackTraceIfDevelopment(context, extensions);
        AddUserClaims(user, extensions);

        return problemDetails;
    }

    private static void AddStackTraceIfDevelopment(HttpContext context, IDictionary<string, object?> extensions)
    {
        var env = context.RequestServices.GetService<IHostEnvironment>();

        if (env is not null && env.IsDevelopment())
        {
            var stackTrace = context.Features.Get<IExceptionHandlerFeature>()?.Error?.StackTrace;

            if (!string.IsNullOrEmpty(stackTrace))
            {
                extensions["stackTrace"] = stackTrace;
            }
        }
    }

    private static void AddUserClaims(ClaimsPrincipal? user, IDictionary<string, object?> extensions)
    {
        if (user?.Identity?.IsAuthenticated is not true)
        {
            return;
        }

        string? userId = null, userName = null;

        foreach (var claim in user.Claims)
        {
            if (userId is null && claim.Type == ClaimTypes.NameIdentifier)
            {
                userId = claim.Value;
            }
            else if (userName is null && claim.Type == ClaimTypes.Name)
            {
                userName = claim.Value;
            }

            if (userId is not null && userName is not null)
            {
                break;
            }
        }

        if (userId is not null)
        {
            extensions["userId"] = userId;
        }

        if (userName is not null)
        {
            extensions["userName"] = userName;
        }
    }

    private static List<object> CreateErrorList(ValidationModelException validationException)
    {
        var errorList = new List<object>();

        foreach (var e in validationException.Errors)
        {
            foreach (var m in e.Value)
            {
                errorList.Add(new { Name = e.Key, Message = m });
            }
        }

        return errorList;
    }

    private static HttpStatusCode GetStatusCodeFromException(Exception exception) => exception switch
    {
        ArgumentOutOfRangeException or ArgumentNullException => HttpStatusCode.BadRequest,
        BadRequestException => HttpStatusCode.BadRequest,
        ConflictException => HttpStatusCode.Conflict,
        NotFoundException => HttpStatusCode.NotFound,
        UnauthorizeException => HttpStatusCode.Unauthorized,
        ValidationModelException => HttpStatusCode.UnprocessableEntity,
        _ => HttpStatusCode.InternalServerError
    };

    private static string GetMessageFromException(Exception exception) => exception switch
    {
        ArgumentOutOfRangeException argumentOutOfRangeException => argumentOutOfRangeException.Message,
        ArgumentNullException argumentNullException => argumentNullException.Message,
        BadRequestException badRequestException => badRequestException.Message,
        ConflictException conflictException => conflictException.Message,
        NotFoundException notFoundException => notFoundException.Message,
        UnauthorizeException => MessagesExceptions.UserNotAuthenticated,
        ValidationModelException validationModelException => validationModelException.Message,
        _ => MessagesExceptions.UnexpectedError
    };
}