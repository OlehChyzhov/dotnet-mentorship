using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Domain.Models;
using Mapster;

namespace Airbnb.Audit.Application.Mapping;

public class ApartmentAuditMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ApartmentAuditEntity => ApartmentAuditDto
        config.NewConfig<ApartmentAuditEntity, ApartmentAuditDto>();
    }
}

public class BookingAuditMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // BookingAuditEntity => BookingAuditDto
        config.NewConfig<BookingAuditEntity, BookingAuditDto>();
    }
}