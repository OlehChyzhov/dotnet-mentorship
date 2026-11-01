using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Payment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, MockPaymentService>();
        return services;
    }
}