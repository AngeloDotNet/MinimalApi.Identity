namespace MinimalApi.Identity.Core.Utility.Messages;

public static class MessagesExceptions
{
    public const string UserNotAuthenticated = "User is not authenticated";
    public const string UserNotHavePermission = "User does not have permission";
    public const string ProblemDetailsMessageTitle = "An error occurred while processing your request";
    public const string UnexpectedError = "An unexpected error occurred!";
    public const string ConstraintViolation = "A violation occurred for a database constraint";
    public const string InvalidAccessToken = "Invalid access token";
    public const string InvalidRefreshToken = "Invalid refresh token";
}
