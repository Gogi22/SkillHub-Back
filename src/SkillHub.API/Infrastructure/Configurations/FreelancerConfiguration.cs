using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class FreelancerConfiguration : BaseEntityTypeConfiguration<Freelancer, string>
{
    public override void Configure(EntityTypeBuilder<Freelancer> builder)
    {
        base.Configure(builder);

        builder.HasMany(f => f.Skills)
            .WithMany();
        builder.HasMany(f => f.Projects)
            .WithOne(p => p.Freelancer)
            .HasForeignKey(p => p.FreelancerId);
    }
}