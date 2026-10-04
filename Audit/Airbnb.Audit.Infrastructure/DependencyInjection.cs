using Airbnb.Audit.Application.Abstracts;
using Airbnb.Audit.Application.Abstracts.Broker;
using Airbnb.Audit.Application.Options;
using Airbnb.Audit.Infrastructure.Broker;
using Airbnb.Audit.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace Airbnb.Audit.Infrastructure;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructureAsync(this IServiceCollection services, IConfiguration configuration)
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
        await channel.QueueDeclareAsync(
            queue: rabbitmqOptions.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        services.AddSingleton<IConnection>(connection);
        services.AddSingleton<IChannel>(channel);
        services.AddSingleton<IEventConsumer, RabbitMqConsumer>();
        
        // MongoDB
        services.AddOptions<MongoDbOptions>().Bind(configuration.GetSection("MongoDb"));
        services.AddSingleton<IMongoClient>(config =>
        {
            return new MongoClient(configuration.GetConnectionString("MongoDb"));
        });
        
        services.AddScoped<MongoDbContext>();
        
        return services;
    }
}