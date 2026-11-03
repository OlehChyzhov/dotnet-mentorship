namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Events;

public class PaymentWebhookTriggeredEvent
{
    public Guid BookingId { get; set; }
    public bool Success { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? PaymentSessionId { get; set; }
    public string? FailureReason { get; set; }
}