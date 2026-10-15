using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Booking;

public record CreateBookingDto
{
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }

    public int GuestsCount { get; init; }
    public Guid ApartmentId { get; init; }
}