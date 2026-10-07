using System.Text;
using Airbnb.Contracts.Broker;
using Airbnb.Messaging.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Airbnb.Messaging.Broker;

public class RabbitMqConsumer : IEventConsumer, IAsyncDisposable
{
    private readonly MessageBrokerOptions _options;
    private readonly IChannel _channel;
    private string? _consumerTag;
    
    public RabbitMqConsumer(
        IOptions<MessageBrokerOptions> options,
        IChannel channel)
    {
        _options = options.Value;
        _channel = channel;
    }

    public async Task StartAsync()
    {
        if (_consumerTag != null)
        {
            return;
        }
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceived;
        
        _consumerTag = await _channel.BasicConsumeAsync(
            queue: _options.Queue,
            autoAck: false,
            consumer: consumer);
    }

    private async Task OnMessageReceived(object sender,  BasicDeliverEventArgs eventArgs)
    {
        string message = Encoding.UTF8.GetString(eventArgs.Body.Span);
        string? messageType = eventArgs.BasicProperties.Type;
        
        Console.WriteLine("Received {0}: {1}", messageType, message);
        
        await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_consumerTag != null && _channel.IsOpen)
        {
            await _channel.BasicCancelAsync(_consumerTag);
        }
    }
}