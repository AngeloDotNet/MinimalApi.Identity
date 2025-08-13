using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class EmailSending : BaseEntity, IEntity
{
    public string EmailTo { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    //public int EmailSendingStatusId { get; set; }
    //public EmailSendingStatus EmailSendingStatus { get; set; } = null!;
    //public int EmailSendingId { get; set; }
    //public EmailSending EmailSending { get; set; } = null!;
    public int TypeEmailSendingId { get; set; }
    public TypeEmailSending TypeEmailSending { get; set; } = null!;
    //public bool Sent { get; set; }
    public int TypeEmailStatusId { get; set; }
    public TypeEmailStatus TypeEmailStatus { get; set; } = null!;
    public DateTime DateSent { get; set; } // Data e ora in cui l'email è stata inviata con successo
    public int RetrySender { get; set; } // Numero di tentativi di invio dell'email - Numero tentativi: 10 (default)
    //public TimeSpan DelayOnError { get; set; } // Tempo di attesa tra i tentativi di invio dell'email in caso di errore - Default: 00:00:30 (30 secondi)
    public DateTime? RetrySenderDate { get; set; } // Data e ora dell'ultimo tentativo di invio dell'email
    public string? RetrySenderErrorMessage { get; set; } //Consente valore NULL
    public string? RetrySenderErrorDetails { get; set; } //Consente valore NULL
}