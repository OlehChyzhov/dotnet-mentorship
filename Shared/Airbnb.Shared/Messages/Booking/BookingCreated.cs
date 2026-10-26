namespace Airbnb.Shared.Messages.Booking;

public record BookingCreated
{
    public string EntityId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}