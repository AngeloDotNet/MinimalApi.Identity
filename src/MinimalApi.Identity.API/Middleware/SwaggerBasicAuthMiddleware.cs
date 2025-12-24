using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Settings;

namespace MinimalApi.Identity.API.Middleware;

public class SwaggerBasicAuthMiddleware(RequestDelegate next, IOptions<SwaggerSettings> settingsOptions)
{
    private readonly SwaggerSettings settings = settingsOptions.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        var isAuthenticationRequired = (context.Request.Path == "/index.html" || context.Request.Path.StartsWithSegments("/swagger"))
            && settings.AuthSettings.UserName.HasValue() && settings.AuthSettings.Password.HasValue();

        if (!isAuthenticationRequired)
        {
            await next.Invoke(context).ConfigureAwait(false);
            return;
        }

        string? authenticationHeader = context.Request.Headers[HeaderNames.Authorization];

        if (authenticationHeader?.StartsWith("Basic ") == true)
        {
            // Get the credentials from request header
            var header = AuthenticationHeaderValue.Parse(authenticationHeader);

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter!)).Split(':', count: 2);
            var userName = credentials.ElementAtOrDefault(0);
            var password = credentials.ElementAtOrDefault(1);

            // validate credentials
            if (userName == settings.AuthSettings.UserName && password == settings.AuthSettings.Password)
            {
                await next.Invoke(context).ConfigureAwait(false);
                return;
            }
        }

        context.Response.Headers.WWWAuthenticate = new StringValues("Basic");
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }
}