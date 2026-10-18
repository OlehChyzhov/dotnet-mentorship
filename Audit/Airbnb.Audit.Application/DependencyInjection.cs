using System.Reflection;
using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Audit.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Mapper
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(assembly);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        // Services
        services.AddScoped<IUserAuditService, UserAuditService>();
        services.AddScoped<IApartmentAuditService, ApartmentAuditService>();
        services.AddScoped<IBookingAuditService, BookingAuditService>();
        
        return services;
    }
}