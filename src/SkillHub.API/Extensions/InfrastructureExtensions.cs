namespace SkillHub.API.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApiDbContext>((serviceProvider, options) =>
        {
            var x = bool.TryParse(configuration["UseInMemoryDatabase"], out var inMemory);
            var interceptor = serviceProvider.GetRequiredService<AuditableEntitiesInterceptor>();
            if (x && !inMemory)
            {
                options.UseSqlServer(configuration["SqlServerConnectionString"]!)
                    .AddInterceptors(interceptor);
            }
            else
            {
                options.UseInMemoryDatabase("MyDB")
                    .AddInterceptors(interceptor);
            }
        });

        services.AddScoped<AuditableEntitiesInterceptor>();

        return services;
    }
}