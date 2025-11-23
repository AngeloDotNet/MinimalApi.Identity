using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.Shared.Results.AspNetCore.Http;

namespace MinimalApi.Identity.Core.Options;

public class ValidationOptions
{
    public ErrorResponseFormat ErrorResponseFormat { get; set; }
    public Func<EndpointFilterInvocationContext, IDictionary<string, string[]>, string>? ValidationErrorTitleMessageFactory { get; set; }
}
