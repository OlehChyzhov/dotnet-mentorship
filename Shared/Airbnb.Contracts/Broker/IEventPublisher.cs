namespace Airbnb.Contracts.Broker;

public interface IEventPublisher
{ 
    Task PublishAsync<TMessage>(TMessage message, byte priority = 0);
}