using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Booking;

public class CreateBookingDto
{
    public DateTime CheckIn { get; set; }
    
    public DateTime CheckOut { get; set; }

    public int GuestsCount { get; set; }
    
    public Guid ApartmentId { get; set; }
}