using System.Text.Json;
using Airbnb.Contracts.Broker;
using Airbnb.Messaging.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Airbnb.Messaging.Broker;

public class RabbitMqPublisher : IEventPublisher
{
    private readonly IChannel _channel;
    private readonly MessageBrokerOptions _options;
    
    public RabbitMqPublisher(IChannel channel, IOptions<MessageBrokerOptions> options)
    {
        _channel = channel;
        _options = options.Value;
    }

    public async Task PublishAsync<TMessage>(TMessage message, byte priority = 0)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var props = new BasicProperties()
        {
            ContentType = "application/json",
            ContentEncoding = "utf-8",
            Persistent = false,
            Priority = priority,
            Type = typeof(TMessage).Name
        };
        
        await _channel.BasicPublishAsync(
            exchange: _options.Exchange,
            routingKey: RoutingKeys.For<TMessage>(), 
            mandatory: true, 
            basicProperties: props,
            body: body);
    }
}