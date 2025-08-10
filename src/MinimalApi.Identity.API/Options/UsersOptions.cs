using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Options;

public class UsersOptions
{
    [Required, EmailAddress]
    public string AssignAdminRoleOnRegistration { get; init; } = null!;

    //[Required, Range(1, 365, ErrorMessage = "PasswordExpirationDays must be greater than zero and less than or equal to 365 (1 year).")]
    [Required]
    public int PasswordExpirationDays { get; init; } = 90; // Default: 90 days
}
