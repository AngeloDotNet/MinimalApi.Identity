using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;

namespace MinimalApi.Identity.Core.Filters;

internal class ValidatorFilter<TModel>(IValidator<TModel> validator, IOptions<ValidationOptions> options) : IEndpointFilter where TModel : class
{
    private readonly ValidationOptions validationOptions = options.Value;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TModel)) is not TModel input)
        {
            return TypedResults.UnprocessableEntity();
        }

        var validationResult = await validator.ValidateAsync(input, context.HttpContext.RequestAborted);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

        var errors = validationResult.ToDictionary();
        var result = TypedResults.Problem(
            statusCode: StatusCodes.Status422UnprocessableEntity,
            instance: context.HttpContext.Request.Path,
            title: validationOptions.ValidationErrorTitleMessageFactory?.Invoke(context, errors) ?? ConstantsConfiguration.ValidationOccurred,
            extensions: new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier,
                ["errors"] = validationOptions.ErrorResponseFormat == ErrorResponseFormat.Default ? errors : errors.SelectMany(e => e.Value.Select(m => new { Name = e.Key, Message = m })).ToArray()
            }
        );

        return result;
    }
}