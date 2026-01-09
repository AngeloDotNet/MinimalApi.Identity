using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;

namespace IdentityManager.API.Middleware;

public class ExtendedExceptionMiddleware(RequestDelegate next, ErrorResponseFormat errorResponseFormat) : MinimalApiExceptionMiddleware(next, errorResponseFormat)
{
    public static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            // Here you can add additional exception cases to handle them specifically

            default:
                problemDetails = CreateProblemDetails(context, HttpStatusCode.InternalServerError, "An unexpected error occurred!");
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}