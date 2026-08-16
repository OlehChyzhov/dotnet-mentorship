using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public BookingStatus Status { get; set; }
    
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    
    public double BookedPricePerNight { get; set; }
    public double BookedTotalPrice { get; set; }

    public int GuestsCount { get; set; }

    public DateTime CreatedAt { get; set; }
}
