using Airbnb.Audit.Domain.Enums;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Messages.User;
using Mapster;

namespace Airbnb.Audit.Application.Mapping.Messages;

public class UserAuditEntityConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserCreated => UserAuditEntity
        config.NewConfig<UserCreated, UserAuditEntity>()
            .Map(dest => dest.UserId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Created)
            .Map(dest => dest.EventOccuredAt, src => src.CreatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
        
        // UserUpdated => UserAuditEntity
        config.NewConfig<UserUpdated, UserAuditEntity>()
            .Map(dest => dest.UserId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Updated)
            .Map(dest => dest.EventOccuredAt, src => src.UpdatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedProperty, src => src.PropertyName)
            .Map(dest => dest.OldValue, src => src.OldValue)
            .Map(dest => dest.NewValue, src => src.NewValue);
        
        // UserDeleted => UserAuditEntity
        config.NewConfig<UserDeleted, UserAuditEntity>()
            .Map(dest => dest.UserId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Deleted)
            .Map(dest => dest.EventOccuredAt, src => src.DeletedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
    }
}