using System.Text;
using System.Text.Json;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Messages;
using Airbnb.Messaging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Airbnb.Messaging.Broker;

public class RabbitMqConsumer : IEventConsumer, IAsyncDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MessageBrokerOptions _options;
    private readonly IChannel _channel;
    private string? _consumerTag;
    
    public RabbitMqConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<MessageBrokerOptions> options,
        IChannel channel)
    {
        _scopeFactory = scopeFactory;
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
        try
        {
            var messageNamespace = $"{typeof(UserCreated).Namespace}.{eventArgs.BasicProperties.Type}";
            var messageType = typeof(UserCreated).Assembly.GetType(messageNamespace, throwOnError: true)!;
            var message = JsonSerializer.Deserialize(eventArgs.Body.Span, messageType);
            
            await using var scope = _scopeFactory.CreateAsyncScope();
            var handlerType = typeof(IMessageService<>).MakeGenericType(messageType);
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);
            
            await (Task)handlerType.GetMethod("HandleAsync")!.Invoke(handler, [message])!;
            await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to handle '{eventArgs.BasicProperties.Type}': {ex}");
            await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_consumerTag != null && _channel.IsOpen)
        {
            await _channel.BasicCancelAsync(_consumerTag);
        }
    }
}