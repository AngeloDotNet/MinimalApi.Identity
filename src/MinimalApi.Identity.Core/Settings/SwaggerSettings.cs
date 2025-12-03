namespace MinimalApi.Identity.Core.Settings;

public class SwaggerSettings
{
    public bool IsEnabled { get; set; }
    public AuthSettings AuthSettings { get; set; } = new AuthSettings();
    //public bool IsRequiredAuth { get; set; }
    //public string? UserName { get; set; }
    //public string? Password { get; set; }
}

public class AuthSettings
{
    //public const string SectionName = "SwaggerSettings";
    //public bool IsEnabled { get; set; }
    public bool IsRequired { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}