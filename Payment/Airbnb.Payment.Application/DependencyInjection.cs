using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.Handlers;
using Airbnb.Payment.Application.Options;
using Airbnb.Payment.Application.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe.Checkout;

namespace Airbnb.Payment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Services
        services.AddScoped<IPaymentService, StripePaymentService>();
        
        // Stripe
        services.Configure<StripeConfig>(configuration.GetSection("Stripe"));
        services.AddScoped<SessionService>();
        
        return services;
    }
}