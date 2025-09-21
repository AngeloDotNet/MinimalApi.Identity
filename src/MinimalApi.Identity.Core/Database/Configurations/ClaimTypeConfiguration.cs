using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.Core.Database.Configurations;

public class ClaimTypeConfiguration : IEntityTypeConfiguration<ClaimType>
{
    public void Configure(EntityTypeBuilder<ClaimType> builder)
    {
        var claimTypeList = new List<ClaimType>();
        var idRiga = 1;

        foreach (var ct in Enum.GetValues<ClaimsType>())
        {
            claimTypeList.Add(new ClaimType
            {
                Id = idRiga,
                Type = nameof(ClaimsType.Permission),
                Value = ct.ToString(),
                Default = true
            });

            idRiga++;
        }

        builder.HasData(claimTypeList);
    }
}