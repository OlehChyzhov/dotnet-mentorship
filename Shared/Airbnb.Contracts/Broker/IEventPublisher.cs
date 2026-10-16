namespace Airbnb.Contracts.Broker;

public interface IEventPublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
}