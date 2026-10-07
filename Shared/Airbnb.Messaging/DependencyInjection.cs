using Airbnb.Contracts.Broker;
using Airbnb.Messaging.Broker;
using Airbnb.Messaging.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Airbnb.Messaging;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        await ConfigureRabbitMqAsync(services, configuration);
        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
        return services;
    }

    public static async Task<IServiceCollection> AddConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        await ConfigureRabbitMqAsync(services, configuration);
        services.AddSingleton<IEventConsumer, RabbitMqConsumer>();
        return services;
    }

    private static async Task<IServiceCollection> ConfigureRabbitMqAsync(this IServiceCollection services, IConfiguration configuration)
    {
        // Message Broker (RabbitMQ)
        var rabbitmqSection = configuration.GetSection("RabbitMq");
        services.Configure<MessageBrokerOptions>(rabbitmqSection);
        
        var rabbitmqOptions = rabbitmqSection.Get<MessageBrokerOptions>()!;
        
        var factory = new ConnectionFactory()
        {
            HostName = rabbitmqOptions.HostName,
            Port = rabbitmqOptions.Port,
            UserName = rabbitmqOptions.UserName,
            Password = rabbitmqOptions.Password,
            ClientProvidedName = rabbitmqOptions.ClientProvidedName
        };
        
        IConnection connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: rabbitmqOptions.Exchange,
            type: ExchangeType.Topic,
            durable: false,
            autoDelete: false);
        
        await channel.QueueDeclareAsync(
            queue: rabbitmqOptions.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await channel.QueueBindAsync(
            queue: rabbitmqOptions.Queue, 
            exchange: rabbitmqOptions.Exchange,
            routingKey: "user.#");
        
        services.AddSingleton<IConnection>(connection);
        services.AddSingleton<IChannel>(channel);
        
        return services;
    }
}