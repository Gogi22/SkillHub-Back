using Microsoft.EntityFrameworkCore;
using SkillHub.API.Entities;

namespace SkillHub.API.Infrastructure;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
        
    }
    
    // public DbSet<Freelancer> Users { get; set; } = null!;
    // public DbSet<Project> Projects { get; set; } = null!;
    // public DbSet<Proposal> Proposals { get; set; } = null!;
    // public DbSet<Review> Reviews { get; set; } = null!;
    
}