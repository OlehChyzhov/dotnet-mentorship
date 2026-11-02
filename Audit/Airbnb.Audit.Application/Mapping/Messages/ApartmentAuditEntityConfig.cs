using Airbnb.Audit.Domain.Enums;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Contracts.Events.Apartment;
using Mapster;

namespace Airbnb.Audit.Application.Mapping.Messages;

public class ApartmentAuditEntityConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ApartmentCreated => ApartmentAuditEntity
        config.NewConfig<ApartmentCreatedEvent, ApartmentAuditEntity>()
            .Map(dest => dest.ApartmentId, src => src.ApartmentId)
            .Map(dest => dest.ChangeType, src => ChangeType.Created)
            .Map(dest => dest.EventOccuredAt, src => src.CreatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
        
        // ApartmentUpdated => ApartmentAuditEntity
        config.NewConfig<ApartmentUpdatedEvent, ApartmentAuditEntity>()
            .Map(dest => dest.ApartmentId, src => src.ApartmentId)
            .Map(dest => dest.ChangeType, src => ChangeType.Updated)
            .Map(dest => dest.EventOccuredAt, src => src.UpdatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedProperty, src => src.PropertyName)
            .Map(dest => dest.OldValue, src => src.OldValue)
            .Map(dest => dest.NewValue, src => src.NewValue);
        
        // ApartmentDeleted => ApartmentAuditEntity
        config.NewConfig<ApartmentDeletedEvent, ApartmentAuditEntity>()
            .Map(dest => dest.ApartmentId, src => src.ApartmentId)
            .Map(dest => dest.ChangeType, src => ChangeType.Deleted)
            .Map(dest => dest.EventOccuredAt, src => src.DeletedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
    }
}