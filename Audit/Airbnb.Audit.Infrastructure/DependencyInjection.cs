using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Infrastructure.Consumers;
using Airbnb.Audit.Infrastructure.Database;
using Airbnb.Audit.Infrastructure.Database.CollectionConfigs;
using Airbnb.Audit.Infrastructure.Database.DbConfigs;
using Airbnb.Audit.Infrastructure.Database.Repositories;
using Airbnb.Audit.Infrastructure.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Message Broker (MassTransit.RabbitMQ)
        var rabbitmqSection = configuration.GetSection("RabbitMq");
        services.Configure<MessageBrokerOptions>(rabbitmqSection);
        var rabbitmqOptions = rabbitmqSection.Get<MessageBrokerOptions>()!;
        
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.AddConsumersFromNamespaceContaining<ApartmentMessageConsumer>();
            
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
        
        // MongoDB Context
        services.AddSingleton<MongoDbContext>(sp =>
        {
            var mongoUrl = MongoUrl.Create(configuration.GetConnectionString("MongoDb"));
            var client = new MongoClient(mongoUrl);
            var db =  client.GetDatabase(mongoUrl.DatabaseName);
            
            return new MongoDbContext(db);
        });

        // MongoDB Configurations
        services.AddHostedService<MongoIndexConfig>();
        
        // Collection Configurations
        BsonClassMap.RegisterClassMap(new UserAuditEntityConfigs());
        BsonClassMap.RegisterClassMap(new ApartmentAuditEntityConfigs());
        BsonClassMap.RegisterClassMap(new BookingAuditEntityConfigs());
        
        services.AddScoped<IUserAuditRepository, UserAuditRepository>();
        services.AddScoped<IApartmentAuditRepository, ApartmentAuditRepository>();
        services.AddScoped<IBookingAuditRepository, BookingAuditRepository>();
        
        return services;
    }
}