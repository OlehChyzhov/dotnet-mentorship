using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Booking;

public record BookingDto
{
    public Guid Id { get; init; }
    public BookingStatus Status { get; init; }
    
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    
    public decimal BookedPricePerNight { get; init; }
    public decimal BookedTotalPrice { get; init; }
    
    public int GuestsCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
