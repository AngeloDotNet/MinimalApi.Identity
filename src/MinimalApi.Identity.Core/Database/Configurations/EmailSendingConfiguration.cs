using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.Core.Database.Configurations;

public class EmailSendingConfiguration : IEntityTypeConfiguration<EmailSending>
{
    public void Configure(EntityTypeBuilder<EmailSending> builder)
    {
        builder.HasOne(builder => builder.TypeEmailSending)
            .WithMany(y => y.EmailSendings)
            .HasForeignKey(y => y.TypeEmailSendingId)
            .IsRequired();

        builder.HasOne(builder => builder.TypeEmailStatus)
            .WithMany(typeEmailStatus => typeEmailStatus.EmailSendings)
            .HasForeignKey(builder => builder.TypeEmailStatusId)
            .IsRequired();
    }
}