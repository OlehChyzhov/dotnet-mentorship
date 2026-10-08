using System.Reflection;
using Airbnb.Audit.Application.Services;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Messages;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Audit.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(assembly);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IMessageService<UserCreated>, UserAuditService<UserCreated>>();
        services.AddScoped<IMessageService<UserDeleted>, UserAuditService<UserDeleted>>();
        services.AddScoped<IMessageService<UserEmailChanged>, UserAuditService<UserEmailChanged>>();

        return services;
    }
}