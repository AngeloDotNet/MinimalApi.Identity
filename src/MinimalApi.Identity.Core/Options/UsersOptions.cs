using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.Core.Options;

public class UsersOptions
{
    [Required]
    public string AssingAdminUsername { get; init; } = null!;

    [Required, EmailAddress]
    public string AssignAdminEmail { get; init; } = null!;

    [Required]
    public string AssignAdminPassword { get; init; } = null!;

    [Required]
    public int PasswordExpirationDays { get; init; } = 90; // Default: 90 days
}