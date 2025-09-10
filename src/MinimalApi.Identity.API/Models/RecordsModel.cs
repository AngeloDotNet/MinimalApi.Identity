namespace MinimalApi.Identity.API.Models;

public record class RegisterModel(string Firstname, string Lastname, string Username, string Email, string Password);
public record class LoginModel(string Username, string Password, bool RememberMe);
public record class ForgotPasswordModel(string Email);
public record class ResetPasswordModel(string Email, string Password, string ConfirmPassword);
public record class RefreshTokenModel(string AccessToken, string RefreshToken);
public record class ImpersonateUserModel(int UserId);

public record class CreateModuleModel(string Name, string Description);
public record class AssignModuleModel(int UserId, int ModuleId);
public record class RevokeModuleModel(int UserId, int ModuleId);
public record class DeleteModuleModel(int ModuleId);

public record class CreateRoleModel(string Role);
public record class AssignRoleModel(string Username, string Role);
public record class RevokeRoleModel(string Username, string Role);
public record class DeleteRoleModel(string Role);

public record class CreateClaimModel(string Type, string Value);
public record class AssignClaimModel(int UserId, string Type, string Value);
public record class RevokeClaimModel(int UserId, string Type, string Value);
public record class DeleteClaimModel(string Type, string Value);