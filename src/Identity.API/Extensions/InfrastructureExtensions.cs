using IdentityServer.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            if (configuration.GetConnectionString("SqlServerConnection") != null)
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")!);
            }
            else
            {
                options.UseInMemoryDatabase("MyDB");
            }
        });
        return services;
    }
}