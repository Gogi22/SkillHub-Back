using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class BaseUserConfiguration : BaseEntityTypeConfiguration<BaseUser, string>
{
    public override void Configure(EntityTypeBuilder<BaseUser> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UserName).IsRequired();
        builder.Property(b => b.Email).IsRequired();
        
        base.Configure(builder);
    }
}