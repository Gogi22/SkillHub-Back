namespace Common;

public interface IEventProducer
{
    void Publish(IIntegrationEvent @event, string queueName);
}