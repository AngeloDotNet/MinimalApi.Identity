using Microsoft.AspNetCore.Http;

namespace MinimalApi.Identity.Core.Utility.Generators;

public static class CallBackGenerator
{
    private const string EndpointsConfirmEmail = "/confirm-email/{userId}/{token}";
    private const string EndpointsConfirmEmailChange = "/confirm-email-change/{userId}/{email}/{token}";

    public static Task<string> GenerateCallBackUrlAsync(string userId, string token, IHttpContextAccessor httpContextAccessor, string newEmail = null!)
    {
        var endpoint = string.Empty;
        var request = httpContextAccessor.HttpContext!.Request;

        if (newEmail == null)
        {
            endpoint = EndpointsConfirmEmailChange
                .Replace("{userId}", userId)
                .Replace("{email}", newEmail)
                .Replace("{token}", token);
        }
        else
        {
            endpoint = EndpointsConfirmEmail
                .Replace("{userId}", userId)
                .Replace("{token}", token);
        }

        var callbackUrl = string.Concat(request.Scheme, "://", request.Host, EndpointGenerator.EndpointsAccountGroup, endpoint);

        return Task.FromResult(callbackUrl);
    }
}