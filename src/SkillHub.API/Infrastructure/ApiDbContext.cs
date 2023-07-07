using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<Freelancer> Freelancers { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;

    public DbSet<Skill> Skills { get; set; } = null!;

    public DbSet<Proposal> Proposals { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseUser>().UseTpcMappingStrategy();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiDbContext).Assembly);
        modelBuilder.Entity<Skill>().HasData(
            new Skill { SkillId = 1, Name = ".Net" },
            new Skill { SkillId = 2, Name = "React" },
            new Skill { SkillId = 3, Name = "Java" }
        );
    }
}