using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Payment.Infrastructure.Handlers;
using Airbnb.Payment.Infrastructure.Options;
using Airbnb.Payment.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe.Checkout;

namespace Airbnb.Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        
        // Message Broker (MassTransit.RabbitMQ)
        var rabbitmqSection = configuration.GetSection("RabbitMq");
        services.Configure<MessageBrokerOptions>(rabbitmqSection);
        var rabbitmqOptions = rabbitmqSection.Get<MessageBrokerOptions>()!;
        
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumersFromNamespaceContaining<CreatePaymentSessionCommandHandler>();
            config.AddConsumersFromNamespaceContaining<CreatePaymentTransactionCommandHandler>();
            
            config.UsingRabbitMq((context, rabbitMqConfig) =>
            {
                rabbitMqConfig.Host(new Uri(rabbitmqOptions.HostUri), hostAddress =>
                {
                    hostAddress.Username(rabbitmqOptions.UserName);
                    hostAddress.Password(rabbitmqOptions.Password);
                });
                
                rabbitMqConfig.UseInMemoryOutbox();
                rabbitMqConfig.ConfigureEndpoints(context);
            });
        });
        
        // Stripe
        services.Configure<StripeConfig>(configuration.GetSection("Stripe"));
        services.AddScoped<SessionService>();
        
        // Services
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddScoped<IPaymentWebhookService, StripePaymentWebhookService>();
        return services;
    }
}