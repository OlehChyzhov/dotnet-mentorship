using System.Text.Json;
using Airbnb.Application.Abstracts.Broker;
using Airbnb.Messaging.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Airbnb.Messaging.Broker.Publisher;

public class RabbitMqPublisher : IEventPublisher
{
    private readonly IChannel _channel;
    private readonly MessageBrokerOptions _options;
    
    public RabbitMqPublisher(IChannel channel, IOptions<MessageBrokerOptions> options)
    {
        _channel = channel;
        _options = options.Value;
    }

    public async Task PublishAsync<TMessage>(TMessage message)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var props = new BasicProperties()
        {
            Persistent = false,
            ContentType = "application/json",
            Type = typeof(TMessage).Name
        };
        
        await _channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: _options.Queue, 
            mandatory: true, 
            basicProperties: props,
            body: body);
    }
}