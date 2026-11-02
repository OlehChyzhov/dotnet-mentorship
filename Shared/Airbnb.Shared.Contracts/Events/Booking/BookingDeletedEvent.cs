namespace Airbnb.Shared.Contracts.Events.Booking;

public record BookingDeletedEvent
{
    public string BookingId { get; set; } =  string.Empty;
    public DateTime DeletedAt { get; set; } =  DateTime.UtcNow;
}