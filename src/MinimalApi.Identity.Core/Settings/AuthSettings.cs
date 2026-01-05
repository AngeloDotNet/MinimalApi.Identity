namespace MinimalApi.Identity.Core.Settings;

public class AuthSettings
{
    public bool IsRequired { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}