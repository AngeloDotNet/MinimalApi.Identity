namespace MinimalApi.Identity.Licenses.Models;

public record class LicenseResponseModel(int Id, string Name, DateOnly ExpirationDate);
