using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.Core.Configurations;

public static class ConstantsConfiguration
{
    public static string WebSiteDev => "https://angelo.aepserver.it/";
    public static string LicenseMIT => "https://opensource.org/licenses/MIT";

    public static string NoActivePoliciesFound => "No active policies found in the database.";
    public static string BadRequest => "Bad Request"; // 400
    public static string Unauthorized => "Unauthorized"; // 401
    public static string NotFound => "Not Found"; // 404
    public static string Conflict => "Conflict"; // 409

    public static DateTime Today => DateTime.UtcNow.Date;
    public static DateOnly DateOnlyToday => DateOnly.FromDateTime(Today);
    public static DateTime DateNull => new DateTime(1900, 1, 1);
    public static DateOnly DateOnlyNull => DateOnly.FromDateTime(DateNull);

    public const string Module = nameof(ClaimsType.Module);

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

    public const string EndpointsCreateClaim = "/create-claim";
    public const string EndpointsAssignClaim = "/assign-claim";
    public const string EndpointsRevokeClaim = "/revoke-claim";
    public const string EndpointsDeleteClaim = "/delete-claim";
}