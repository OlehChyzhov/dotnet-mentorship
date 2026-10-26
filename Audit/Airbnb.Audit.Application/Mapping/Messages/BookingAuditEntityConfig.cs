using Airbnb.Audit.Domain.Enums;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Messages.Booking;
using Mapster;

namespace Airbnb.Audit.Application.Mapping.Messages;

public class BookingAuditEntityConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // BookingCreated => BookingAuditEntity
        config.NewConfig<BookingCreated, BookingAuditEntity>()
            .Map(dest => dest.BookingId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Created)
            .Map(dest => dest.EventOccuredAt, src => src.CreatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
        
        // BookingUpdated => BookingAuditEntity
        config.NewConfig<BookingUpdated, BookingAuditEntity>()
            .Map(dest => dest.BookingId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Updated)
            .Map(dest => dest.EventOccuredAt, src => src.UpdatedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedProperty, src => src.PropertyName)
            .Map(dest => dest.OldValue, src => src.OldValue)
            .Map(dest => dest.NewValue, src => src.NewValue);
        
        // BookingDeleted => BookingAuditEntity
        config.NewConfig<BookingDeleted, BookingAuditEntity>()
            .Map(dest => dest.BookingId, src => src.EntityId)
            .Map(dest => dest.ChangeType, src => ChangeType.Deleted)
            .Map(dest => dest.EventOccuredAt, src => src.DeletedAt)
            .Map(dest => dest.AuditCreatedAt, src => DateTime.UtcNow);
    }
}