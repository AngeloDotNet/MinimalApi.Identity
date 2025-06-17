using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.Core.Database.Configurations;

public class UserLicenseConfiguration : IEntityTypeConfiguration<UserLicense>
{
    public void Configure(EntityTypeBuilder<UserLicense> builder)
    {
        builder.HasKey(ul => new { ul.UserId, ul.LicenseId });

        builder.HasOne(ul => ul.User)
            .WithMany(u => u.UserLicenses).HasForeignKey(ul => ul.UserId).IsRequired();

        builder.HasOne(ul => ul.License)
            .WithMany(l => l.UserLicenses).HasForeignKey(ul => ul.LicenseId).IsRequired();
    }
}
