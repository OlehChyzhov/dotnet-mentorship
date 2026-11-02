namespace Airbnb.Shared.Contracts.Events.Payment;

public class PaymentSucceededEvent
{
    public Guid BookingId { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public long AmountTotal { get; set; }
}