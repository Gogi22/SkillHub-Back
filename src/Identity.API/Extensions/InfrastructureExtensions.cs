using Identity.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            var x = bool.TryParse(configuration["UseInMemoryDatabase"], out var inMemory);
            if (x && !inMemory)
            {
                options.UseSqlServer(configuration["SqlServerConnectionString"]!);
            }
            else
            {
                options.UseInMemoryDatabase("MyDB");
            }
        });
        return services;
    }
}