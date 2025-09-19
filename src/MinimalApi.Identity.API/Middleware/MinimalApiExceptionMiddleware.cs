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
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.API.Middleware;

public class MinimalApiExceptionMiddleware(RequestDelegate next, IOptions<ValidationOptions> options)
{
    private readonly ValidationOptions validationOptions = options.Value;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, validationOptions).ConfigureAwait(false);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ValidationOptions validationOptions)
    {
        var statusCode = GetStatusCodeFromException(exception);
        var message = GetMessageFromException(exception);
        var problemDetails = CreateProblemDetails(context, statusCode, message);

        if (exception is ValidationModelException validationException)
        {
            if (validationOptions.ErrorResponseFormat == ErrorResponseFormat.List)
            {
                var errorList = new List<object>();
                foreach (var e in validationException.Errors)
                {
                    foreach (var m in e.Value)
                    {
                        errorList.Add(new { Name = e.Key, Message = m });
                    }
                }

                problemDetails.Extensions["errors"] = errorList;
            }
            else
            {
                problemDetails.Extensions["errors"] = validationException.Errors;
            }
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

        var env = context.RequestServices.GetService<IHostEnvironment>();
        if (env is not null && env.IsDevelopment())
        {
            var stackTrace = context.Features.Get<IExceptionHandlerFeature>()?.Error?.StackTrace;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                extensions["stackTrace"] = stackTrace;
            }
        }

        if (user?.Identity?.IsAuthenticated == true)
        {
            // Avoid ToDictionary allocation if not needed
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
                    break;
            }

            if (userId is not null)
                extensions["userId"] = userId;

            if (userName is not null)
                extensions["userName"] = userName;
        }

        return problemDetails;
    }

    private static HttpStatusCode GetStatusCodeFromException(Exception exception)
        => exception switch
        {
            ArgumentOutOfRangeException or ArgumentNullException => HttpStatusCode.BadRequest,
            BadRequestException => HttpStatusCode.BadRequest,
            ConflictException => HttpStatusCode.Conflict,
            NotFoundException => HttpStatusCode.NotFound,
            UnauthorizeException => HttpStatusCode.Unauthorized,
            ValidationModelException => HttpStatusCode.UnprocessableEntity,
            _ => HttpStatusCode.InternalServerError
        };

    private static string GetMessageFromException(Exception exception)
        => exception switch
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