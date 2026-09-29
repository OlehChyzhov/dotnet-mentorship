using System.Text;
using Airbnb.Application.Abstracts.Broker;
using Airbnb.Application.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Airbnb.Infrastructure.Broker;

public class RabbitMqPublisher : IEventPublisher
{
    private readonly IChannel _channel;
    private readonly MessageBrokerOptions _options;
    
    public RabbitMqPublisher(IChannel channel, IOptions<MessageBrokerOptions> options)
    {
        _channel = channel;
        _options = options.Value;
    }

    public async Task PublishAsync<TMessage>()
    {
        var message = "Test";
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: _options.Queue, 
            mandatory: true, 
            basicProperties: 
            new BasicProperties() { Persistent = true }, 
            body: body);
    }
}