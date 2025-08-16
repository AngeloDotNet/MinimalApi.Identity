namespace MinimalApi.Identity.Core.Utility.Messages;

public static class MessagesAPI
{
    // Claim messages
    public const string ClaimCreated = "Claim created successfully";
    public const string ClaimNotFound = "Claim not found";
    public const string ClaimNotDeleted = "Claim not deleted, it is not possible to delete a claim created by default";
    public const string ClaimDeleted = "Claim deleted successfully";
    public const string ClaimTypeInvalid = "Claim type is invalid";
    public const string ClaimAlreadyAssigned = "Claim already assigned";
    public const string ClaimAssigned = "Claim assigned successfully";
    public const string ClaimNotAssigned = "Claim not assigned";
    public const string ClaimRevoked = "Claim revoked successfully";
    public const string ClaimNotRevoked = "Claim not revoked";
    public const string ClaimsNotFound = "Claims not found";
    public const string ClaimsNotAssigned = "Claims not assigned";
    public const string ClaimAlreadyExist = "Claim already exists";

    // Confirmed messages
    public const string ConfirmingEmail = "Thank you for confirming your email";
    public const string ConfirmingEmailChanged = "Thank you for confirming your email change";

    // Email messages
    public const string EmailCreated = "Email created successfully";
    public const string EmailNotFound = "Email not found";
    public const string EmailUpdated = "Email updated successfully";
    public const string EmailDeleted = "Email deleted successfully";

    // Error messages
    public const string ErrorChangeUsername = "Error changing username";
    public const string ErrorEmailNotConfirmed = "Email not confirmed";
    public const string ErrorCodeResetPassword = "A code must be supplied for password reset.";
    public const string ErrorConfirmEmailChange = "Error changing email";
    public const string ErrorConfirmEmail = "Error confirming your email";

    // Generic messages
    public const string InvalidCredentials = "Invalid credentials";
    public const string ResetPassword = "Password reset successfully";

    // License messages
    public const string LicenseCreated = "License created successfully";
    public const string LicenseNotFound = "License not found";
    public const string LicenseAssigned = "License assigned successfully";
    public const string LicenseCanceled = "License removed successfully";
    public const string LicensesNotFound = "Licenses not found";
    public const string LicenseDeleted = "License deleted successfully";
    public const string LicenseNotDeleted = "License not deleted, it is not possible to delete a license assigned to a user";
    public const string LicenseNotAssignable = "You cannot assign more than one license to a user. Please check which license the user owns.";
    public const string LicenseExpired = "License expired. Without a valid license you cannot use the software.";
    public const string LicenseAlreadyExist = "License already exists";

    // Module messages
    public const string ModuleCreated = "Module created successfully";
    public const string ModuleNotFound = "Module not found";
    public const string ModuleAssigned = "Module assigned successfully";
    public const string ModuleCanceled = "Module removed successfully";
    public const string ModulesNotFound = "Modules not found";
    public const string ModuleDeleted = "Module deleted successfully";
    public const string ModuleNotDeleted = "Module not deleted, it is not possible to delete a module assigned to a user";
    public const string ModuleNotAssignable = "You cannot assign this module to the user as they already own it.";
    public const string ModuleAlreadyExist = "Module already exists";

    // Policy messages
    public const string PolicyNotFound = "Policy not found";
    public const string PolicyAlreadyExist = "Policy already exists";
    public const string PolicyCreated = "Policy created successfully";
    public const string PolicyNotDeleted = "Policy not deleted, it is not possible to delete a policy created by default";
    public const string PolicyDeleted = "Policy deleted successfully";

    // Profile messages
    public const string ProfileCreated = "Profile created successfully";
    public const string ProfileUpdated = "Profile updated successfully";
    public const string ProfileNotFound = "Profile not found";
    public const string ProfileNotCreated = "Profile not created";
    public const string ProfileNotUpdated = "Profile not updated";
    public const string ProfileDeleted = "Profile deleted successfully";
    public const string ProfileDisabled = "Profile disabled successfully";
    public const string ProfileEnabled = "Profile enabled successfully";
    public const string ProfileNotDisabled = "Profile not disabled";
    public const string ProfileNotEnabled = "Profile not enabled";
    public const string ProfilesNotFound = "Profiles not found";

    // Required messages
    public const string RequiredTwoFactor = "Two-factor authentication is required";
    public const string RequiredNewEmail = "New email is required";

    // Role messages
    public const string RoleCreated = "Role created successfully";
    public const string RoleNotFound = "Role not found";
    public const string RoleAssigned = "Role assigned successfully";
    public const string RoleCanceled = "Role removed successfully";
    public const string RoleDeleted = "Role deleted successfully";
    public const string RoleNotDeleted = "Role not deleted, it is not possible to delete a role created by default";
    public const string RoleExists = "Role already exists";
    public const string RolesNotFound = "Roles not found";
    public const string RoleNotAssigned = "Role not assigned";

    // Send messages
    public const string SendEmailResetPassword = "Please check your email to reset your password";
    public const string SendEmailForChangeEmail = "Confirmation link to change email sent. Please check your email";

    // User messages
    public const string UserCreated = "User created successfully";
    public const string UserForcedChangePassword = "The user must change the password, otherwise it will not be possible to sign in.";
    public const string UserIdEmailTokenRequired = "UserId, Email and Token are required";
    public const string UserIdTokenRequired = "UserId and Token are required";
    public const string UserLockedOut = "This account has been locked out, please try again later";
    public const string UserLogOut = "User logged out";
    public const string UserNotAllowedLogin = "User is not allowed to sign in";
    public const string UserNotEmailConfirmed = "User is not email confirmed";
    public const string UserNotEnableLogin = "User is not allowed to sign in";
    public const string UserNotFound = "User not found";
}