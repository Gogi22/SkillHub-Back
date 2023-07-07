using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure.Configurations;

public class ClientConfiguration : BaseEntityTypeConfiguration<Client, string>
{
    public override void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(c => c.WebsiteUrl)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(c => c.CompanyName)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.ClientInfo)
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty);


        builder.HasMany(c => c.Projects)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}