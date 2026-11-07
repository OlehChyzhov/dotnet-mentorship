using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Payment.Infrastructure.Database.Repositories;
using Airbnb.Payment.Infrastructure.Options;
using Airbnb.Payment.Infrastructure.Saga.BookingPayment;
using Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;
using Airbnb.Payment.Infrastructure.Saga.BookingPayment.Success;
using Airbnb.Payment.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;
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
            
            config.AddConsumersFromNamespaceContaining<CreatePaymentTransactionCommandHandler>();
            config.AddConsumersFromNamespaceContaining<InvalidatePaymentSessionCommandHandler>();
            
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
        
        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

        // Stripe
        services.Configure<StripeConfig>(configuration.GetSection("Stripe"));
        services.AddScoped<SessionService>();
        services.AddScoped<RefundService>();
        
        // Services
        services.AddSingleton<IStripeClient>(sp =>
        {
            var stripeConfig = sp.GetRequiredService<IOptions<StripeConfig>>().Value;
            return new StripeClient(stripeConfig.SecretKey);
        });
        services.AddScoped<IPaymentWebhookService, StripePaymentWebhookService>();
        return services;
    }
}