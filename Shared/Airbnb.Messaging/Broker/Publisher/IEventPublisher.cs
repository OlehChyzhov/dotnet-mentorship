namespace Airbnb.Messaging.Broker.Publisher;

public interface IEventPublisher
{ 
    Task PublishAsync<TMessage>(TMessage message, byte priority = 0);
}