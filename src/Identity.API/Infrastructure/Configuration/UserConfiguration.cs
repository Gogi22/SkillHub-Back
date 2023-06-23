using IdentityServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Infrastructure.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users")
            .HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
        builder.Property(p => p.UserName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.PasswordHash)
            .IsRequired();
        builder.Property(p => p.PasswordHash)
            .IsRequired();
        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(p => p.Claims);

        builder.HasIndex(p => p.UserName).IsUnique();
        builder.HasIndex(p => p.Email).IsUnique();
    }
}