namespace MinimalApi.Identity.API.Constants;

public static class EndpointsApi
{
    public const string EndpointsAuthRegister = "/register";
    public const string EndpointsAuthLogin = "/login";
    public const string EndpointsAuthRefreshToken = "/refresh-token";
    public const string EndpointsAuthLogout = "/logout";
    public const string EndpointsForgotPassword = "/forgot-password";
    public const string EndpointsResetPassword = "/reset-password/{code}";
    public const string EndpointsImpersonateUser = "/impersonate-user";

    public const string EndpointsCreateModule = "/create-module";
    public const string EndpointsAssignModule = "/assign-module";
    public const string EndpointsRevokeModule = "/revoke-module";
    public const string EndpointsDeleteModule = "/delete-module";

    public const string EndpointsCreateRole = "/create-role";
    public const string EndpointsAssignRole = "/assign-role";
    public const string EndpointsRevokeRole = "/revoke-role";
    public const string EndpointsDeleteRole = "/delete-role";

    public const string EndpointsCreateClaim = "/create-claim";
    public const string EndpointsAssignClaim = "/assign-claim";
    public const string EndpointsRevokeClaim = "/revoke-claim";
    public const string EndpointsDeleteClaim = "/delete-claim";
}