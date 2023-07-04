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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiDbContext).Assembly);
    }
}