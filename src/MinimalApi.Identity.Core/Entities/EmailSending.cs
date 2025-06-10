using MinimalApi.Identity.Core.Entities.Common;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.Core.Entities;

public class EmailSending : BaseEntity, IEntity
{
    public EmailSendingType EmailSendingType { get; set; }
    public string EmailTo { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool Sent { get; set; }
    public DateTime DateSent { get; set; }
    public string? ErrorMessage { get; set; } //Consente valore NULL
    public string? ErrorDetails { get; set; } //Consente valore NULL
}