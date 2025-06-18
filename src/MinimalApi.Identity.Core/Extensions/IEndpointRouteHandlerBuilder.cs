using Microsoft.AspNetCore.Routing;

namespace MinimalApi.Identity.Core.Extensions;

public interface IEndpointRouteHandlerBuilder
{
    public static abstract void MapEndpoints(IEndpointRouteBuilder endpoints);
}