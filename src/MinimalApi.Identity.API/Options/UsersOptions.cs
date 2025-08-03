using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Options;

public class UsersOptions
{
    [Required, EmailAddress(ErrorMessage = "Email is required.")]
    public string AssignAdminRoleOnRegistration { get; init; } = null!;

    [Required, Range(1, 365, ErrorMessage = "PasswordExpirationDays must be greater than zero and less than or equal to 365 (1 year).")]
    public int PasswordExpirationDays { get; init; }
}
