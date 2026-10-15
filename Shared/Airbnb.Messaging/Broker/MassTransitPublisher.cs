using Airbnb.Contracts.Broker;
using MassTransit;

namespace Airbnb.Messaging.Broker;

public class MassTransitPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public MassTransitPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task PublishAsync<TMessage>(TMessage message)
    {
        if (message == null)
        {
            return;
        }
        
        await _publishEndpoint.Publish(message);
    }
}