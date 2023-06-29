using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using SkillHub.API.Infrastructure;
using SkillHub.API.Services;

namespace SkillHub.API.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApiDbContext>(options =>
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

        services.AddHostedService<RegisteredUserEventConsumer>(provider => new RegisteredUserEventConsumer(provider,
            new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"],
                Password = configuration["RabbitMQ:Password"],
                UserName = configuration["RabbitMQ:UserName"],
                VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/"
            }
        ));
        
        return services;
    }
}