using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Broker.Messages.User;
using Airbnb.Messaging.Broker;
using Airbnb.Messaging.Broker.Consumers;
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

    public static async Task<IServiceCollection> AddConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        await ConfigureRabbitMqAsync(services, configuration);
        
        services.AddSingleton<IEventConsumer, RabbitMqConsumer<UserCreated>>();
        services.AddSingleton<IEventConsumer, RabbitMqConsumer<UserDeleted>>();
        services.AddSingleton<IEventConsumer, RabbitMqConsumer<UserEmailChanged>>();
        
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
        
        services.AddSingleton<IConnection>(connection);
        services.AddSingleton<IChannel>(channel);
        
        return services;
    }
}