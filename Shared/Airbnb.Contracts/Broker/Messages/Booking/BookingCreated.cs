namespace Airbnb.Contracts.Broker.Messages.Booking;

public record BookingCreated
{
    public string EntityId { get; set; } = string.Empty;
}