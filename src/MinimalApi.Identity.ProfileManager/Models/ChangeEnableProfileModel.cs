namespace MinimalApi.Identity.ProfileManager.Models;

public record class ChangeEnableProfileModel(int UserId, bool IsEnabled);