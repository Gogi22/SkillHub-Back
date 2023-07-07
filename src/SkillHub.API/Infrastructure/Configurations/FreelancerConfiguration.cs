using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class FreelancerConfiguration : BaseEntityTypeConfiguration<Freelancer, string>
{
    public override void Configure(EntityTypeBuilder<Freelancer> builder)
    {
        builder.Property(p => p.Bio)
            .HasMaxLength(600)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(p => p.ProfilePhotoId)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(f => f.Skills)
            .WithMany();
    }
}