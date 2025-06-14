namespace MinimalApi.Identity.Licenses.Models;

public record class CreateLicenseModel(string Name, DateOnly ExpirationDate);
