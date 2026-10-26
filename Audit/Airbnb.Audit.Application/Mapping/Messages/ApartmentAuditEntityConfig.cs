using Airbnb.Audit.Domain.Enums;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts.Broker.Messages.Apartment;
using Mapster;

namespace Airbnb.Audit.Application.Mapping.Messages;

public class ApartmentAuditEntityConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ApartmentCreated => ApartmentAuditEntity
        config.NewConfig<ApartmentCreated, ApartmentAuditEntity>()
            .Map(dest => dest.ApartmentId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Created)
            .Map(dest => dest.EventOccuredAt, src => src.CreatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
        
        // ApartmentUpdated => ApartmentAuditEntity
        config.NewConfig<ApartmentUpdated, ApartmentAuditEntity>()
            .Map(dest => dest.ApartmentId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Updated)
            .Map(dest => dest.EventOccuredAt, src => src.UpdatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedProperty, src => src.PropertyName)
            .Map(dest => dest.OldValue, src => src.OldValue)
            .Map(dest => dest.NewValue, src => src.NewValue);
        
        // ApartmentDeleted => ApartmentAuditEntity
        config.NewConfig<ApartmentDeleted, ApartmentAuditEntity>()
            .Map(dest => dest.ApartmentId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Deleted)
            .Map(dest => dest.EventOccuredAt, src => src.DeletedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
    }
}