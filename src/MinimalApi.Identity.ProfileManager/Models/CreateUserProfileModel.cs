namespace MinimalApi.Identity.ProfileManager.Models;

public record class CreateUserProfileModel(int UserId, string FirstName, string LastName);