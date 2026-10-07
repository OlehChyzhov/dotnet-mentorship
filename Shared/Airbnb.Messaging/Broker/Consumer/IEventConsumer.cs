namespace Airbnb.Messaging.Broker.Consumer;

public interface IEventConsumer
{
    Task StartAsync();
}