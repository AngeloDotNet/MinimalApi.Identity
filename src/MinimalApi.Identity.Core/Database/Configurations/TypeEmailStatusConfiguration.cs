using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.Core.Database.Configurations;

public class TypeEmailStatusConfiguration : IEntityTypeConfiguration<TypeEmailStatus>
{
    public void Configure(EntityTypeBuilder<TypeEmailStatus> builder)
    {
        builder.HasData(
            new TypeEmailStatus
            {
                Id = 1,
                Name = "Sent",
                Description = "Email has been sent successfully"
            },
            new TypeEmailStatus
            {
                Id = 2,
                Name = "Pending",
                Description = "Email is pending to be sent"
            },
            new TypeEmailStatus
            {
                Id = 3,
                Name = "Failed",
                Description = "Email sending failed"
            },
            new TypeEmailStatus
            {
                Id = 4,
                Name = "Cancelled",
                Description = "Email sending has been cancelled"
            }
        );
    }
}
