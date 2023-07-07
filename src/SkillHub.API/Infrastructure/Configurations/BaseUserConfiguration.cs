using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class BaseUserConfiguration :  IEntityTypeConfiguration<BaseUser>
{
    public void Configure(EntityTypeBuilder<BaseUser> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.UserName)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(b => b.FirstName)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(b => b.LastName)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(b => b.Email)
            .HasMaxLength(70)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt).IsRequired();
        
        builder.Property(e => e.ModifiedAt).IsRequired();
    }
}