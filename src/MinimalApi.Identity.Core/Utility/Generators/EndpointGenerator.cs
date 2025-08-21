namespace MinimalApi.Identity.Core.Utility.Generators;

public static class EndpointGenerator
{
    private const string EndpointsDefaultApi = "/api";
    public const string EndpointsStringEmpty = "";

    // Endpoints shared
    public const string EndpointsConfirmEmail = "/confirm-email/{userId}/{token}";
    public const string EndpointsConfirmEmailChange = "/confirm-email-change/{userId}/{email}/{token}";

    // Endpoints groups
    public const string EndpointsAccountGroup = EndpointsDefaultApi + "/account";
    public const string EndpointsAuthGroup = EndpointsDefaultApi + "/auth";
    public const string EndpointsClaimsGroup = EndpointsDefaultApi + "/claims";
    public const string EndpointsLicenseGroup = EndpointsDefaultApi + "/licenses";
    public const string EndpointsModulesGroup = EndpointsDefaultApi + "/modules";
    public const string EndpointsAuthPolicyGroup = EndpointsDefaultApi + "/policy";
    public const string EndpointsProfilesGroup = EndpointsDefaultApi + "/profiles";
    public const string EndpointsRolesGroup = EndpointsDefaultApi + "/roles";

    // Endpoints tags
    public const string EndpointsAccountTag = "Account";
    public const string EndpointsAuthTag = "Authentication";
    public const string EndpointsClaimsTag = "Claims";
    public const string EndpointsLicenseTag = "Licenses";
    public const string EndpointsModulesTag = "Modules";
    public const string EndpointsAuthPolicyTag = "Policies";
    public const string EndpointsProfilesTag = "Profiles";
    public const string EndpointsRolesTag = "Roles";
}