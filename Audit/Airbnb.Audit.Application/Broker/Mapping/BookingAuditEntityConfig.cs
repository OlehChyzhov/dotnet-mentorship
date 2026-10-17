using Airbnb.Contracts.Broker.Messages.Booking;
using Mapster;

namespace Airbnb.Audit.Application.Broker.Mapping;

public class BookingAuditEntityConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // BookingCreated => BookingAuditEntityConfig
        config.NewConfig<BookingCreated, BookingAuditEntityConfig>();
        
        // BookingUpdated => BookingAuditEntityConfig
        config.NewConfig<BookingUpdated, BookingAuditEntityConfig>();
        
        // BookingDeleted => BookingAuditEntityConfig
        config.NewConfig<BookingDeleted, BookingAuditEntityConfig>();
    }
}