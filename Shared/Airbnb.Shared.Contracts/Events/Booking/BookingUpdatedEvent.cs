namespace Airbnb.Shared.Contracts.Events.Booking;

public record BookingUpdatedEvent
{
    public string BookingId { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } =  DateTime.UtcNow;
}