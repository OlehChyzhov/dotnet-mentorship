namespace Airbnb.Contracts.Broker;

public interface IEventConsumer
{
    Task StartAsync();
}