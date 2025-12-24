using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Utility.Generators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Identity.Core.Filters;

public class ModulesManagementSwaggerFilter(bool enabledFeatureModule) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!enabledFeatureModule)
        {
            var toRemove = swaggerDoc.Paths.Keys
                .Where(path => path.StartsWith(EndpointGenerator.EndpointsModulesGroup, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}