using Airbnb.Audit.Domain.Enums;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts.Broker.Messages.User;
using Mapster;

namespace Airbnb.Audit.Application.Broker.Mapping;

public class MessagesToUserAuditEntityMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserCreated => UserAuditChangeEntity
        config.NewConfig<UserCreated, UserAuditChangeEntity>()
            .Map(dest => dest.ChangeType, src => ChangeType.UserCreated);
        
        // UserDeleted => UserAuditChangeEntity
        config.NewConfig<UserDeleted, UserAuditChangeEntity>()
            .Map(dest => dest.ChangeType, src => ChangeType.UserDeleted);

        // UserEmailChanged => UserAuditChangeEntity
        config.NewConfig<UserEmailChanged, UserAuditChangeEntity>()
            .Map(dest => dest.ChangeType, src => ChangeType.EmailChanged)
            .Map(dest => dest.OldValue, src => src.OldEmail)
            .Map(dest => dest.Email, src => src.NewEmail);
    }
}