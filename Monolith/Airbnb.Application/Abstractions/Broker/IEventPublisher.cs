namespace Airbnb.Application.Abstracts.Broker;

public interface IEventPublisher
{ 
    Task PublishAsync<TMessage>(TMessage message);
}