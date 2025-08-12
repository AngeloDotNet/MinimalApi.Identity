namespace MinimalApi.Identity.Core.Utility.Messages;

public static class MessagesExceptions
{
    // User exceptions
    public const string UserNotAuthenticated = "User is not authenticated";
    public const string UserNotHavePermission = "User does not have permission";

    // General exceptions
    public const string ProblemDetailsMessageTitle = "An error occurred while processing your request";
    public const string UnexpectedError = "An unexpected error occurred!";

    // Database exceptions
    public const string ConstraintViolation = "A violation occurred for a database constraint";

    // Authentication exceptions
    public const string InvalidAccessToken = "Invalid access token";
    public const string InvalidRefreshToken = "Invalid refresh token";
}
