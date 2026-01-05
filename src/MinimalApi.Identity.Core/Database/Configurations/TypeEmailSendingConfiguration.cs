using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.Core.Database.Configurations;

public class TypeEmailSendingConfiguration : IEntityTypeConfiguration<TypeEmailSending>
{
    public void Configure(EntityTypeBuilder<TypeEmailSending> builder)
    {
        builder.HasData(
            new TypeEmailSending
            {
                Id = 1,
                Name = "RegisterUser",
                Description = "Email sent during user registration"
            },
            new TypeEmailSending
            {
                Id = 2,
                Name = "ChangeEmail",
                Description = "Email sent for changing email address"
            },
            new TypeEmailSending
            {
                Id = 3,
                Name = "ForgotPassword",
                Description = "Email sent for password reset requests"
            }
        );
    }
}