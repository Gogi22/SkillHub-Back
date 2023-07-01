using System.Text;
using Common.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SkillHub.API.Services;

public class RegisteredUserEventConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly IServiceProvider _serviceProvider;

    public RegisteredUserEventConsumer(IServiceProvider serviceProvider, ConnectionFactory connectionFactory,
        string queueName)
    {
        _serviceProvider = serviceProvider;
        _queueName = queueName;
        _channel = connectionFactory.CreateConnection().CreateModel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueDeclare(_queueName, true, false, false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += OnMessageReceived;

        _channel.BasicConsume(_queueName,
            false,
            consumer);

        return Task.CompletedTask;
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs ea)
    {
        try
        {
            // using var scope = _serviceProvider.CreateScope();
            // var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            ProcessMessage(null, ea.Body.ToArray());
            _channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception)
        {
            _channel.BasicNack(ea.DeliveryTag, false, true);
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