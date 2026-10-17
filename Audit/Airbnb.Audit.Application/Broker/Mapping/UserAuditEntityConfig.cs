using Airbnb.Contracts.Broker.Messages.User;
using Mapster;

namespace Airbnb.Audit.Application.Broker.Mapping;

public class UserAuditEntityConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserCreated => UserAuditEntityConfig
        config.NewConfig<UserCreated, UserAuditEntityConfig>();
        
        // UserUpdated => UserAuditEntityConfig
        config.NewConfig<UserUpdated, UserAuditEntityConfig>();
        
        // UserDeleted => UserAuditEntityConfig
        config.NewConfig<UserDeleted, UserAuditEntityConfig>();
    }
}