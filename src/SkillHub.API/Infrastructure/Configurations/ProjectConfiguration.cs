using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class ProjectConfiguration : BaseEntityTypeConfiguration<Project, int>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Budget)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientId);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.ExperienceLevel)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany(p => p.Proposals)
            .WithOne(pr => pr.Project)
            .HasForeignKey(pr => pr.ProjectId);

        builder.HasMany(p => p.Skills)
            .WithMany();
    }
}