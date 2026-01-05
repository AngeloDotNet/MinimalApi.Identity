namespace MinimalApi.Identity.API.Models;

public record class RegisterModel(string Firstname, string Lastname, string Username, string Email, string Password);