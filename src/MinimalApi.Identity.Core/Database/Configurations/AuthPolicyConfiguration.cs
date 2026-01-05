using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.Core.Database.Configurations;

public class AuthPolicyConfiguration : IEntityTypeConfiguration<AuthPolicy>
{
    public void Configure(EntityTypeBuilder<AuthPolicy> builder)
    {
        var policies = new AuthPolicy[]
        {
            new()
            {
                Id = 1,
                PolicyName = nameof(Permissions.AuthPolicyRead),
                PolicyDescription = nameof(Permissions.AuthPolicyRead),
                PolicyPermissions = [nameof(Permissions.AuthPolicy), nameof(Permissions.AuthPolicyRead)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 2,
                PolicyName = nameof(Permissions.AuthPolicyWrite),
                PolicyDescription = nameof(Permissions.AuthPolicyWrite),
                PolicyPermissions = [nameof(Permissions.AuthPolicy), nameof(Permissions.AuthPolicyWrite)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 3,
                PolicyName = nameof(Permissions.ClaimRead),
                PolicyDescription = nameof(Permissions.ClaimRead),
                PolicyPermissions = [nameof(Permissions.Claim), nameof(Permissions.ClaimRead)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 4,
                PolicyName = nameof(Permissions.ClaimWrite),
                PolicyDescription = nameof(Permissions.ClaimWrite),
                PolicyPermissions = [nameof(Permissions.Claim), nameof(Permissions.ClaimWrite)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 5,
                PolicyName = nameof(Permissions.LicenzaRead),
                PolicyDescription = nameof(Permissions.LicenzaRead),
                PolicyPermissions = [nameof(Permissions.Licenza), nameof(Permissions.LicenzaRead)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 6,
                PolicyName = nameof(Permissions.LicenzaWrite),
                PolicyDescription = nameof(Permissions.LicenzaWrite),
                PolicyPermissions = [nameof(Permissions.Licenza), nameof(Permissions.LicenzaWrite)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 7,
                PolicyName = nameof(Permissions.ModuloRead),
                PolicyDescription = nameof(Permissions.ModuloRead),
                PolicyPermissions = [nameof(Permissions.Modulo), nameof(Permissions.ModuloRead)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 8,
                PolicyName = nameof(Permissions.ModuloWrite),
                PolicyDescription = nameof(Permissions.ModuloWrite),
                PolicyPermissions = [nameof(Permissions.Modulo), nameof(Permissions.ModuloWrite)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 9,
                PolicyName = nameof(Permissions.ProfiloRead),
                PolicyDescription = nameof(Permissions.ProfiloRead),
                PolicyPermissions = [nameof(Permissions.Profilo), nameof(Permissions.ProfiloRead)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 10,
                PolicyName = nameof(Permissions.ProfiloWrite),
                PolicyDescription = nameof(Permissions.ProfiloWrite),
                PolicyPermissions = [nameof(Permissions.Profilo), nameof(Permissions.ProfiloWrite)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 11,
                PolicyName = nameof(Permissions.RuoloRead),
                PolicyDescription = nameof(Permissions.RuoloRead),
                PolicyPermissions = [nameof(Permissions.Ruolo), nameof(Permissions.RuoloRead)],
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                Id = 12,
                PolicyName = nameof(Permissions.RuoloWrite),
                PolicyDescription = nameof(Permissions.RuoloWrite),
                PolicyPermissions = [nameof(Permissions.Ruolo), nameof(Permissions.RuoloWrite)],
                IsDefault = true,
                IsActive = true
            }
        };

        builder.HasData(policies);
    }
}