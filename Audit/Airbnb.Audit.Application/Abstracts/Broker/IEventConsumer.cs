namespace Airbnb.Audit.Application.Abstracts.Broker;

public interface IEventConsumer
{
    Task StartAsync();
}