using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Utility.Generators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Identity.Core.Filters;

public class LicensesManagementSwaggerFilter(bool enabledFeatureLicense) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!enabledFeatureLicense)
        {
            var toRemove = swaggerDoc.Paths.Keys
                .Where(path => path.StartsWith(EndpointGenerator.EndpointsLicenseGroup, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}