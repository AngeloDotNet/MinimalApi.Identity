using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.Core.Models;

namespace MinimalApi.Identity.Core.Utility.Generators;

public static class CallBackGenerator
{
    public static Task<string> GenerateCallBackUrlAsync(GenerateCallBackUrlModel request, IHttpContextAccessor httpContextAccessor)
    {
        var endpoint = string.Empty;
        var httpRequest = httpContextAccessor.HttpContext!.Request;

        if (request.NewEmail == null)
        {
            endpoint = EndpointGenerator.EndpointsConfirmEmailChange
                .Replace("{userId}", request.UserId)
                .Replace("{email}", request.NewEmail)
                .Replace("{token}", request.Token);
        }
        else
        {
            endpoint = EndpointGenerator.EndpointsConfirmEmail
                .Replace("{userId}", request.UserId)
                .Replace("{token}", request.Token);
        }

        var endpointUrl = string.Concat(EndpointGenerator.EndpointsAccountGroup, endpoint);
        var callbackUrl = string.Concat(httpRequest.Scheme, "://", httpRequest.Host, endpointUrl);
        //var callbackUrl = string.Concat(httpRequest.Scheme, "://", httpRequest.Host, EndpointGenerator.EndpointsAccountGroup, endpoint);

        return Task.FromResult(callbackUrl);
    }
}