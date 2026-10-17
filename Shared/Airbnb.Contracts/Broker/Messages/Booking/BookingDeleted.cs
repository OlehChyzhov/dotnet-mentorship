namespace Airbnb.Contracts.Broker.Messages.Booking;

public record BookingDeleted
{
    public string EntityId { get; set; } =  string.Empty;
}