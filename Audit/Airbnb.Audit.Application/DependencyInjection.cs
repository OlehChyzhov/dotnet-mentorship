using System.Reflection;
using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.Broker;
using Airbnb.Audit.Application.Broker.Services;
using Airbnb.Audit.Application.Services;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Broker.Messages;
using Airbnb.Contracts.Broker.Messages.User;
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

        // Validation
        services.AddValidatorsFromAssembly(assembly);

        // Mapper
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(assembly);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        // Message handlers
        services.AddScoped<IMessageSaver<UserCreated>, UserAuditMessageSaver<UserCreated>>();
        services.AddScoped<IMessageSaver<UserDeleted>, UserAuditMessageSaver<UserDeleted>>();
        services.AddScoped<IMessageSaver<UserEmailChanged>, UserAuditMessageSaver<UserEmailChanged>>();

        // Services
        services.AddScoped<IUserAuditService, UserAuditService>();
        
        return services;
    }
}