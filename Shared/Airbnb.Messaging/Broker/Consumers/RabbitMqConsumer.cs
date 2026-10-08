using System.Text.Json;
using Airbnb.Contracts.Broker;
using Airbnb.Messaging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Airbnb.Messaging.Broker.Consumers;

public class RabbitMqConsumer<TMessage> : IEventConsumer
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
        
        var routingKey = RoutingKeys.For<TMessage>();
        var queue = $"{_options.Queue}.{routingKey}";

        await _channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false);
        await _channel.QueueBindAsync(queue, _options.Exchange, routingKey);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceived;
        
        _consumerTag = await _channel.BasicConsumeAsync(
            queue: queue,
            autoAck: false,
            consumer: consumer);
    }

    private async Task OnMessageReceived(object sender,  BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var message = JsonSerializer.Deserialize<TMessage>(eventArgs.Body.Span);
            await using var scope = _scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageService<TMessage>>();

            var result = await handler.HandleAsync(message);

            if (result.IsSuccessful)
            {
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            else
            {
                Console.WriteLine($"Handler rejected '{typeof(TMessage).Name}'");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to handle '{typeof(TMessage).Name}': {ex}");
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