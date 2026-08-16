using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Booking;

public class CreateBookingDto
{
    public BookingStatus Status { get; set; }
    
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    
    public double BookedPricePerNight { get; set; }
    public double BookedTotalPrice { get; set; }

    public int GuestsCount { get; set; }

    public DateTime CreatedAt { get; set; }
}