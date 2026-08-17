using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Booking;

public record BookingDto
{
    public Guid Id { get; init; }
    public BookingStatus Status { get; init; }
    
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    
    public double BookedPricePerNight { get; init; }
    public double BookedTotalPrice { get; init; }
    
    public int GuestsCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
