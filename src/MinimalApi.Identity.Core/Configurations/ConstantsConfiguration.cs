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
    public static DateTime DateNull => new DateTime(1900, 1, 1);
    public static DateOnly DateOnlyNull => DateOnly.FromDateTime(DateNull);
}