namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;

public class FinishPaymentTransactionCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
    public bool IsSuccessful { get; set; }
}