namespace MinimalApi.Identity.Core.Settings;

public class AppSettings
{
    public string AssignAdminUsername { get; init; } = null!;
    public string AssignAdminEmail { get; init; } = null!;
    public string AssignAdminPassword { get; init; } = null!;
    public int PasswordExpirationDays { get; init; } = 90;
    public int IntervalEmailSenderMinutes { get; init; } = 5;
    public string ErrorResponseFormat { get; init; } = "Default";
    public bool EnabledFeatureLicense { get; init; } = true;
    public bool EnabledFeatureModule { get; init; } = true;
    public int ValidateMinLength { get; init; } = 3;
    public int ValidateMaxLength { get; init; } = 50;
    public int ValidateMinLengthDescription { get; init; } = 5;
    public int ValidateMaxLengthDescription { get; init; } = 100;
    public string DatabaseType { get; init; } = null!;
    public string MigrationsAssembly { get; init; } = null!;
}