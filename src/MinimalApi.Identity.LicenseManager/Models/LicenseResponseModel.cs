namespace MinimalApi.Identity.LicenseManager.Models;

public record class LicenseResponseModel(int Id, string Name, DateOnly ExpirationDate);
