using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Domain.Models;
using Mapster;

namespace Airbnb.Application.Mapping;

public class BookingMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Booking => BookingDto
        config.NewConfig<Booking, BookingDto>();
        
        // CreateBookingDto => Booking
        config.NewConfig<CreateBookingDto, Booking>();
    }
}