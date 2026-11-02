namespace Airbnb.Shared.Contracts.Events.Payment;

public class PaymentFailedEvent
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}