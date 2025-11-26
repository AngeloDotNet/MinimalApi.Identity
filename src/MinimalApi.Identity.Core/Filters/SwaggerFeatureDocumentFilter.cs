using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Utility.Generators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Identity.Core.Filters;

public class SwaggerFeatureDocumentFilter(FeatureFlagsOptions featureFlagsOptions, Func<FeatureFlagsOptions, bool> isFeatureEnabled,
    string endpointGroup) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!isFeatureEnabled(featureFlagsOptions))
        {
            var toRemove = swaggerDoc.Paths.Keys.Where(path
                => path.StartsWith(endpointGroup, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}

//public class LicensesManagementSwaggerFilter(FeatureFlagsOptions featureFlagsOptions) : IDocumentFilter
public class LicensesManagementSwaggerFilter(bool enabledFeatureLicense) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!enabledFeatureLicense)
        {
            var toRemove = swaggerDoc.Paths.Keys.Where(path
                => path.StartsWith(EndpointGenerator.EndpointsLicenseGroup, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}

//public class ModulesManagementSwaggerFilter(FeatureFlagsOptions featureFlagsOptions) : IDocumentFilter
public class ModulesManagementSwaggerFilter(bool enabledFeatureModule) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!enabledFeatureModule)
        {
            var toRemove = swaggerDoc.Paths.Keys.Where(path
                => path.StartsWith(EndpointGenerator.EndpointsModulesGroup, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}

//public class SwaggerFeatureDocumentFilter : IDocumentFilter
//{
//    private readonly FeatureFlagsOptions featureFlagsOptions;
//    private readonly Func<FeatureFlagsOptions, bool> isFeatureEnabled;
//    private readonly string endpointGroup;

//    public SwaggerFeatureDocumentFilter()
//    {
//        featureFlagsOptions = new FeatureFlagsOptions();
//        isFeatureEnabled = _ => true;
//        endpointGroup = string.Empty;
//    }

//    public SwaggerFeatureDocumentFilter(FeatureFlagsOptions featureFlagsOptions, Func<FeatureFlagsOptions, bool> isFeatureEnabled, string endpointGroup)
//    {
//        this.featureFlagsOptions = featureFlagsOptions;
//        this.isFeatureEnabled = isFeatureEnabled;
//        this.endpointGroup = endpointGroup;
//    }

//    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
//    {
//        if (!isFeatureEnabled(featureFlagsOptions))
//        {
//            var toRemove = swaggerDoc.Paths.Keys
//                .Where(path => path.StartsWith(endpointGroup, StringComparison.OrdinalIgnoreCase))
//                .ToList();

//            foreach (var path in toRemove)
//            {
//                swaggerDoc.Paths.Remove(path);
//            }
//        }
//    }
//}