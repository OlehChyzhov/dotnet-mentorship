using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;

namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;

public class PaymentWebhookTriggeredEvent
{
    public Guid BookingId { get; set; }
    public string PaymentIntentId { get; set; }
    public string PaymentSessionId { get; set; }
    public bool IsSuccessful { get; set; }
    public PaymentStatus Status { get; set; }
}