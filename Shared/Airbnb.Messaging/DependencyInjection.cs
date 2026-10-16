using Airbnb.Contracts.Broker;
using Airbnb.Messaging.Broker;
using Airbnb.Messaging.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services, 
        IConfiguration configuration, 
        Action<IBusRegistrationConfigurator>? registerConsumers = null)
    {
        var rabbitmqSection = configuration.GetSection("RabbitMq");
        services.Configure<MessageBrokerOptions>(rabbitmqSection);
        var rabbitmqOptions = rabbitmqSection.Get<MessageBrokerOptions>()!;
        
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            registerConsumers?.Invoke(config);
            
            config.UsingRabbitMq((context, rabbitMqConfig) =>
            {
                rabbitMqConfig.Host(new Uri(rabbitmqOptions.HostUri), hostAddress =>
                {
                    hostAddress.Username(rabbitmqOptions.UserName);
                    hostAddress.Password(rabbitmqOptions.Password);
                });
                
                rabbitMqConfig.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, MassTransitPublisher>();
        
        return services;
    }
}