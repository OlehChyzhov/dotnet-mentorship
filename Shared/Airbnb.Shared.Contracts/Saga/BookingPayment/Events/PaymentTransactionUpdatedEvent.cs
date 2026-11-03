namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Events;

public class PaymentTransactionUpdatedEvent
{
    public Guid BookingId { get; set; }
    public bool IsFailed { get; set; }
    public bool IsSuccessful { get; set; }
    public bool IsExpired { get; set; }
}