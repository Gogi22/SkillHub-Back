using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class ProposalConfiguration : BaseEntityTypeConfiguration<Proposal, int>
{
    public override void Configure(EntityTypeBuilder<Proposal> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.CoverLetter)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(p => p.Freelancer)
            .WithMany()
            .HasForeignKey(p => p.FreelancerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}