using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class ReviewConfiguration : BaseEntityTypeConfiguration<Review, int>
{
    public override void Configure(EntityTypeBuilder<Review> builder)
    {
        base.Configure(builder);
        builder.ToTable("reviews");
        builder.Property(r => r.ReviewText).IsRequired().HasMaxLength(2000);
    }
}