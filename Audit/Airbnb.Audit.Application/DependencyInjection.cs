using Airbnb.Audit.Application.Services;
using Airbnb.Contracts.MessageHandlers;
using Airbnb.Contracts.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Audit.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler<UserCreated>, UserAuditMessageHandler>();
        services.AddScoped<IMessageHandler<UserDeleted>, UserAuditMessageHandler>();
        services.AddScoped<IMessageHandler<UserEmailChanged>, UserAuditMessageHandler>();
        
        return services;
    }
}