using Airbnb.Audit.Application.Services;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Audit.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageService<UserCreated>, UserAuditService<UserCreated>>();
        services.AddScoped<IMessageService<UserDeleted>, UserAuditService<UserDeleted>>();
        services.AddScoped<IMessageService<UserEmailChanged>, UserAuditService<UserEmailChanged>>();
        
        return services;
    }
}