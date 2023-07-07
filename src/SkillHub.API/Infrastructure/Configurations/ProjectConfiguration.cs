using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class ProjectConfiguration : BaseEntityTypeConfiguration<Project, int>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        base.Configure(builder);
        
        builder.Property(p => p.Title)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.Budget)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.ExperienceLevel)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(p => p.Freelancer)
            .WithMany(p => p.Projects)
            .HasForeignKey(p => p.FreelancerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Proposals)
            .WithOne(pr => pr.Project)
            .HasForeignKey(pr => pr.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.Review)
            .WithOne(p => p.Project)
            .HasForeignKey<Review>(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Skills)
            .WithMany();
    }
}