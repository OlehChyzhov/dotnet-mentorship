namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Events;

public class PaymentTransactionCreatedEvent
{
    public Guid BookingId { get; set; }
    public bool IsCreated { get; set; }
}