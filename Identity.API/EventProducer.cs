using System.Text;
using System.Text.Json;
using Common;
using RabbitMQ.Client;

namespace IdentityServer;

public class EventProducer : IDisposable, IEventProducer
{
    private readonly IConnection _connection;

    public EventProducer(ConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection() ??
                      throw new ArgumentNullException(nameof(connectionFactory));
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    public void Publish(IIntegrationEvent @event, string queueName)
    {
        if (null == @event)
            throw new ArgumentNullException(nameof(@event));

        var serializedJson = JsonSerializer.Serialize(@event, @event.GetType());
        var data = Encoding.UTF8.GetBytes(serializedJson);

        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queueName, true, false);

        channel.BasicPublish(string.Empty, queueName, body: data);
    }
}