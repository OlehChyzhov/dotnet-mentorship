using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.Abstractions.Services;
using Airbnb.Payment.Application.Mapping;
using Airbnb.Payment.Application.Services;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Payment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IPaymentService, PaymentService>();

        // Mapping
        TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
        var applicationAssembly = typeof(PaymentTransactionMappingConfig).Assembly;
        TypeAdapterConfig.GlobalSettings.Scan(applicationAssembly);
        services.AddMapster();

        return services;
    }
}
