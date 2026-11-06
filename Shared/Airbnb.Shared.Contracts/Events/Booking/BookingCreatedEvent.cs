namespace Airbnb.Shared.Contracts.Events.Booking;

public record BookingCreatedEvent
{
    public Guid BookingId { get; set; }
    public decimal TotalAmount { get; set; }
    public int GuestCount { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}