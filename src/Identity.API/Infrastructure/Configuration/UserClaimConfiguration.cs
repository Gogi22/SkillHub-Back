using Common;
using Identity.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.API.Infrastructure.Configuration;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaims");
        builder.HasKey(uc => uc.Id);
        builder.Property(uc => uc.UserId).IsRequired();
        builder.Property(uc => uc.ClaimType).HasMaxLength(256).IsRequired();
        builder.Property(uc => uc.ClaimValue)
            .HasConversion(
                s => s.ToString(), // Enum to string conversion
                s => (Role)Enum.Parse(typeof(Role), s) // String to enum conversion
            ).IsRequired();

        builder.HasIndex(uc => new { uc.UserId, uc.ClaimType }).IsUnique();
        builder.HasIndex(uc => uc.UserId);
        builder.HasOne<User>()
            .WithMany(o => o.Claims)
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();
    }
}