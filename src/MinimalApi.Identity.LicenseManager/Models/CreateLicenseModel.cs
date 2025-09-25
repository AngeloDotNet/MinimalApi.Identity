namespace MinimalApi.Identity.LicenseManager.Models;

public record class CreateLicenseModel(string Name, DateOnly ExpirationDate);
