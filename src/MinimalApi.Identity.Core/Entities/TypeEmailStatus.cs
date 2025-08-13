using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class TypeEmailStatus : BaseEntity, IEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<EmailSending> EmailSendings { get; set; } = [];
}
