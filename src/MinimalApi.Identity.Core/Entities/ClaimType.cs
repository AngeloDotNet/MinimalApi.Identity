using MinimalApi.Identity.Core.Entities.Common;

namespace MinimalApi.Identity.Core.Entities;

public class ClaimType : BaseEntity, IEntity
{
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool Default { get; set; }
}