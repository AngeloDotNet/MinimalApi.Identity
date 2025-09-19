namespace MinimalApi.Identity.Core.Settings;

public class AppSettings
{
    public string DatabaseType { get; set; } = null!;
    public string MigrationsAssembly { get; set; } = null!;
    public string AssignAdminUsername { get; set; } = null!;
    public string AssignAdminEmail { get; set; } = null!;
    public string AssignAdminPassword { get; set; } = null!;
    public int PasswordExpirationDays { get; set; } = 90;
    public int IntervalEmailSenderMinutes { get; set; } = 5;
    public string ErrorResponseFormat { get; set; } = "Default";
    public bool EnabledFeatureLicense { get; set; } = true;
    public bool EnabledFeatureModule { get; set; } = true;
    public int ValidateMinLength { get; set; } = 3;
    public int ValidateMaxLength { get; set; } = 50;
    public int ValidateMinLengthDescription { get; set; } = 5;
    public int ValidateMaxLengthDescription { get; set; } = 100;
}