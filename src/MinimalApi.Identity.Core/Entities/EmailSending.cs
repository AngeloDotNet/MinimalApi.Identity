using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class EmailSending : BaseEntity, IEntity
{
    public string EmailTo { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int TypeEmailSendingId { get; set; }
    public TypeEmailSending TypeEmailSending { get; set; } = null!;
    public int TypeEmailStatusId { get; set; }
    public TypeEmailStatus TypeEmailStatus { get; set; } = null!;
    public DateTime DateSent { get; set; }
    public int RetrySender { get; set; }
    public DateTime? RetrySenderDate { get; set; }
    public string? RetrySenderErrorMessage { get; set; } //Consente valore NULL
    public string? RetrySenderErrorDetails { get; set; } //Consente valore NULL
}