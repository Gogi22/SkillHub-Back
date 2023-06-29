using System.Text;
using Common;
using Common.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SkillHub.API.Infrastructure;

namespace SkillHub.API.Services;

public class RegisteredUserEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IModel _channel;

    public RegisteredUserEventConsumer(IServiceProvider serviceProvider, ConnectionFactory connectionFactory)
    {
        _serviceProvider = serviceProvider;
        _channel = connectionFactory.CreateConnection().CreateModel();
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueDeclare("registered_users", true, false, false);

        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += OnMessageReceived;

        _channel.BasicConsume(queue: "registered_users",
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs ea)
    {
        try
        {
            // using var scope = _serviceProvider.CreateScope();
            // var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            ProcessMessage(null, ea.Body.ToArray());
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch (Exception)
        {
            _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
        }
    }
    
    private static void ProcessMessage(ApiDbContext dbContext, byte[] body)
    {
        var message = JsonConvert.DeserializeObject<UserRegisteredEvent>(Encoding.UTF8.GetString(body));
        if (message is null)
        {
            return;
        }

        if (message.Role == Role.Freelancer)
        {
            Console.WriteLine("New freelancer registered");
        }
        else // Role.Client
        {
            Console.WriteLine("New client registered");
        }
    }
    
    public override void Dispose()
    {
        _channel.Dispose();
        base.Dispose();
    }
}