namespace MinimalApi.Identity.Core.Enums;

public enum EmailSendingType
{
    RegisterUser = 1, // Indica che l'email è stata inviata per la registrazione di un nuovo utente
    ChangeEmail = 2, // Indica che l'email è stata inviata per il cambio dell'email di un utente
    ForgotPassword = 3 // Indica che l'email è stata inviata per il ripristino della password di un utente
}
