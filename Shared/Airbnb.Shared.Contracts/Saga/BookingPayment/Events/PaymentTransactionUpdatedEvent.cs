namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Events;

public class PaymentTransactionUpdatedEvent
{
    public Guid BookingId { get; set; }
    public bool Success { get; set; }
}