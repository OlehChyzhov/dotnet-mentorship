namespace Airbnb.Contracts.Broker;

public interface IEventConsumer : IAsyncDisposable
{
    Task StartAsync();
}