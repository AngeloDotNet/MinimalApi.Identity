using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.Core.Database.Configurations;

public class EmailSendingConfiguration : IEntityTypeConfiguration<EmailSending>
{
    public void Configure(EntityTypeBuilder<EmailSending> builder)
    {
        builder.Property(x => x.EmailSendingType).HasConversion<string>();
    }
}
