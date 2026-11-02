namespace Airbnb.Shared.Contracts.Events.Booking;

public record BookingCreatedEvent
{
    public Guid BookingId { get; set; }
    public double BookingTotal { get; set; }
    public int GuestsCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}