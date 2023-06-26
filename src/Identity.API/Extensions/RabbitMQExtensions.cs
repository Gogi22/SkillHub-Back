using Common;
using RabbitMQ.Client;

namespace IdentityServer.Extensions;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventProducer>(_ => new EventProducer(new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"],
            Password = configuration["RabbitMQ:Password"],
            UserName = configuration["RabbitMQ:UserName"],
            VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/"
        }));
        return services;
    }
}