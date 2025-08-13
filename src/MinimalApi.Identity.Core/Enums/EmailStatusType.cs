namespace MinimalApi.Identity.Core.Enums;

public enum EmailStatusType
{
    Sent = 1, // Indica che l'email è stata inviata con successo
    Pending = 2, // Indica che l'email è in attesa di essere inviata
    Failed = 3, // Indica che l'invio dell'email è fallito ma i tentativi non sono finiti
    Cancelled = 4 // Indica che l'invio dell'email è stato annullato causa superamento del numero di tentativi
}