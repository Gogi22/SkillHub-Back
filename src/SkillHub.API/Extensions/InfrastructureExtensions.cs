using RabbitMQ.Client;
using SkillHub.API.Services;

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

        // services.AddHostedService<RegisteredUserEventConsumer>(provider => new RegisteredUserEventConsumer(provider,
        //     new ConnectionFactory
        //     {
        //         HostName = configuration["RabbitMQ:Host"],
        //         Password = configuration["RabbitMQ:Password"],
        //         UserName = configuration["RabbitMQ:UserName"],
        //         VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/"
        //     },
        //     configuration["RabbitMQ:Queues:UserRegistered"]
        // ));

        return services;
    }
}