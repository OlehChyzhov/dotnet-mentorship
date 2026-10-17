namespace Airbnb.Contracts.Broker.Messages.Booking;

public record BookingUpdated
{
    public string EntityId { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
}