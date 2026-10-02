namespace Airbnb.Audit.Application.Abstracts;

public interface IEventConsumer
{
    Task StartAsync();
}