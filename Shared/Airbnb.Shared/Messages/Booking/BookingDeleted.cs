namespace Airbnb.Shared.Messages.Booking;

public record BookingDeleted
{
    public string EntityId { get; set; } =  string.Empty;
    public DateTime DeletedAt { get; set; } =  DateTime.UtcNow;
}