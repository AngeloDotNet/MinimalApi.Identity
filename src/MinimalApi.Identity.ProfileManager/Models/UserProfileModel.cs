namespace MinimalApi.Identity.ProfileManager.Models;

public record class UserProfileModel(int UserId, string Email, string FirstName, string LastName, bool IsEnabled, DateOnly? LastDateChangePassword)
{
    public string FullName => $"{FirstName} {LastName}";
}