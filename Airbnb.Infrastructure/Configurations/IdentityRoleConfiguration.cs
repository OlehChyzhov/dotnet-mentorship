using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.Infrastructure.Configurations;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "1a5e0000-0000-0000-0000-000000000001",
                Name = "Client",
                NormalizedName = "CLIENT",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000001"
            },
            new IdentityRole
            {
                Id = "1a5e0000-0000-0000-0000-000000000002",
                Name = "Host",
                NormalizedName = "HOST",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000002"
            },
            new IdentityRole
            {
                Id = "1a5e0000-0000-0000-0000-000000000003",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000003"
            });
    }
}