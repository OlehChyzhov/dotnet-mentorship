namespace Airbnb.Shared.Broker;

public interface IPublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
}